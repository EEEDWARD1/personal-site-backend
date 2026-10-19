import assert from "node:assert/strict";
import { test } from "node:test";
import { ADMIN_USER_ID, isAdminUser } from "../lib/access.ts";
import { forwardPostRequest } from "../lib/post-proxy-core.ts";

const id = "11111111-1111-1111-1111-111111111111";
const defaults = {
  userId: ADMIN_USER_ID,
  getToken: async () => "server-session-token",
  baseUrl: "https://api.example.com/v1",
  fetcher: (async () => new Response(null, { status: 204 })) as typeof fetch,
};
const write = (method: string, headers = {}) => new Request(`http://localhost:3000/api/posts/${id}`, {
  method, headers: { origin: "http://localhost:3000", "content-type": "application/json", ...headers },
  body: method === "PUT" ? '{"title":"Changed"}' : undefined,
});

test("only the exact owner ID is authorized", () => {
  assert.equal(isAdminUser(ADMIN_USER_ID), true);
  for (const value of [null, undefined, "", "user_other", "user_3KvCdrD7lHa2pwct9Ou7gQbB4NP", ADMIN_USER_ID + " "])
    assert.equal(isAdminUser(value), false);
});

test("anonymous and other users cannot reach the upstream or retrieve a token", async () => {
  for (const [userId, status] of [[null, 401], ["user_other", 403]] as const) {
    const response = await forwardPostRequest(new Request("http://localhost:3000/api/posts"), undefined, {
      ...defaults, userId,
      getToken: async () => { throw new Error("Must not retrieve token"); },
      fetcher: async () => { throw new Error("Must not fetch"); },
    });
    assert.equal(response.status, status);
  }
});

test("list forwards only pagination and uses the server session token", async () => {
  const response = await forwardPostRequest(new Request("http://localhost:3000/api/posts?page=2&pageSize=20&url=https://evil.test", {
    headers: { authorization: "Bearer attacker-token", cookie: "private-cookie" },
  }), undefined, {
    ...defaults,
    fetcher: async (url, init) => {
      assert.equal(String(url), "https://api.example.com/v1/admin/posts?page=2&pageSize=20");
      const headers = new Headers(init?.headers);
      assert.equal(headers.get("authorization"), "Bearer server-session-token");
      assert.equal(headers.has("cookie"), false);
      assert.equal(init?.cache, "no-store");
      assert.equal(init?.redirect, "error");
      return Response.json({ items: [], totalCount: 0 });
    },
  });
  assert.equal(response.status, 200);
  assert.equal(response.headers.get("cache-control"), "no-store");
});

test("ETag and validation/conflict errors pass through without leaking headers", async () => {
  for (const status of [200, 400, 404, 409, 412, 428]) {
    const response = await forwardPostRequest(new Request(`http://localhost:3000/api/posts/${id}`), id, {
      ...defaults,
      fetcher: async () => Response.json({ title: "Test" }, {
        status, headers: { ETag: '"123"', "Set-Cookie": "private-cookie" },
      }),
    });
    assert.equal(response.status, status);
    assert.equal(response.headers.get("etag"), '"123"');
    assert.equal(response.headers.has("set-cookie"), false);
    assert.deepEqual(await response.json(), { title: "Test" });
  }
});

test("update forwards JSON and the exact If-Match version", async () => {
  const response = await forwardPostRequest(write("PUT", { "if-match": '"123"' }), id, {
    ...defaults,
    fetcher: async (url, init) => {
      assert.equal(String(url), `https://api.example.com/v1/admin/posts/${id}`);
      assert.equal(init?.method, "PUT");
      assert.equal(init?.body, '{"title":"Changed"}');
      assert.equal(new Headers(init?.headers).get("if-match"), '"123"');
      return new Response(null, { status: 204 });
    },
  });
  assert.equal(response.status, 204);
  assert.equal(await response.text(), "");
});

test("writes reject foreign or missing origins", async () => {
  for (const origin of ["https://evil.test", "null", ""]) {
    for (const method of ["PUT", "DELETE"]) {
      const response = await forwardPostRequest(write(method, { origin }), id, defaults);
      assert.equal(response.status, 403);
    }
  }
});

test("create forwards only to the fixed posts endpoint", async () => {
  const request = new Request("http://localhost:3000/api/posts", {
    method: "POST", headers: { origin: "http://localhost:3000", "content-type": "application/json" },
    body: '{"slug":"new-post"}',
  });
  const response = await forwardPostRequest(request, undefined, {
    ...defaults,
    fetcher: async (url, init) => {
      assert.equal(String(url), "https://api.example.com/v1/admin/posts");
      assert.equal(init?.method, "POST");
      assert.equal(init?.body, '{"slug":"new-post"}');
      return Response.json({ id, title: "New post" }, { status: 201 });
    },
  });
  assert.equal(response.status, 201);
  assert.deepEqual(await response.json(), { id, title: "New post" });
});

test("delete forwards the GUID without a body", async () => {
  const response = await forwardPostRequest(write("DELETE"), id, {
    ...defaults,
    fetcher: async (url, init) => {
      assert.equal(String(url), `https://api.example.com/v1/admin/posts/${id}`);
      assert.equal(init?.method, "DELETE");
      assert.equal(init?.body, undefined);
      return new Response(null, { status: 204 });
    },
  });
  assert.equal(response.status, 204);
});

test("writes reject non-JSON content", async () => {
  assert.equal((await forwardPostRequest(write("PUT", { "content-type": "text/plain" }), id, defaults)).status, 415);
});

test("rejects arbitrary paths and unsupported methods", async () => {
  const request = new Request("http://localhost:3000/api/posts");
  for (const invalid of ["../tags", "https://evil.test", "a-slug"])
    assert.equal((await forwardPostRequest(request, invalid, defaults)).status, 400);
  assert.equal((await forwardPostRequest(write("DELETE"), undefined, defaults)).status, 405);
});

test("expired session is rejected", async () => {
  const response = await forwardPostRequest(new Request("http://localhost:3000/api/posts"), undefined, {
    ...defaults, getToken: async () => null,
  });
  assert.equal(response.status, 401);
});

test("refuses plaintext remote APIs", async () => {
  const response = await forwardPostRequest(new Request("http://localhost:3000/api/posts"), undefined, {
    ...defaults, baseUrl: "http://remote.example.com/v1",
  });
  assert.equal(response.status, 500);
});

test("allows the exact private Compose API origin", async () => {
  let called = false;
  const response = await forwardPostRequest(new Request("http://localhost:3000/api/posts"), undefined, {
    ...defaults, baseUrl: "http://api:8080/v1",
    fetcher: async url => {
      assert.equal(String(url), "http://api:8080/v1/admin/posts");
      called = true;
      return Response.json({ items: [] });
    },
  });
  assert.equal(response.status, 200);
  assert.equal(called, true);
});

test("does not permit arbitrary internal HTTP destinations", async () => {
  for (const baseUrl of ["http://api:9000/v1", "http://api.evil.test:8080/v1", "http://db:5432/v1"]) {
    const response = await forwardPostRequest(new Request("http://localhost:3000/api/posts"), undefined, {
      ...defaults, baseUrl,
    });
    assert.equal(response.status, 500);
  }
});

test("container writes check the configured browser origin, not the internal listener", async () => {
  const request = new Request(`http://0.0.0.0:3000/api/posts/${id}`, {
    method: "PUT", headers: { origin: "http://localhost:3000", "content-type": "application/json" },
    body: "{}",
  });
  assert.equal((await forwardPostRequest(request, id, { ...defaults, origin: "http://localhost:3000" })).status, 204);
  const foreign = new Request(`http://0.0.0.0:3000/api/posts/${id}`, {
    method: "DELETE", headers: { origin: "http://0.0.0.0:3000" },
  });
  assert.equal((await forwardPostRequest(foreign, id, { ...defaults, origin: "http://localhost:3000" })).status, 403);
});

test("HTTPS tunnel writes use the public origin even though the container uses HTTP", async () => {
  const request = new Request(`http://cms:3000/api/posts/${id}`, {
    method: "PUT", headers: { origin: "https://cms.eduardteodor.co.uk", "content-type": "application/json" }, body: "{}",
  });
  const options = { ...defaults, origin: "https://cms.eduardteodor.co.uk" };
  assert.equal((await forwardPostRequest(request, id, options)).status, 204);
  const foreign = new Request(`http://cms:3000/api/posts/${id}`, {
    method: "DELETE", headers: { origin: "http://localhost:3000" },
  });
  assert.equal((await forwardPostRequest(foreign, id, options)).status, 403);
});

test("upstream failure returns a useful error without a token or stack trace", async () => {
  const response = await forwardPostRequest(new Request("http://localhost:3000/api/posts"), undefined, {
    ...defaults, fetcher: async () => { throw new Error("server-session-token"); },
  });
  assert.equal(response.status, 502);
  assert.ok(!(await response.text()).includes("server-session-token"));
});
