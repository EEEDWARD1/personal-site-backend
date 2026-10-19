import assert from "node:assert/strict";
import { test } from "node:test";
import { resolveCmsOrigin } from "../lib/cms-config.ts";

test("production trusts only the public HTTPS CMS origin by default", () => {
  assert.equal(resolveCmsOrigin("production"), "https://cms.eduardteodor.co.uk");
});

test("development uses localhost unless explicitly overridden", () => {
  assert.equal(resolveCmsOrigin("development"), "http://localhost:3000");
  assert.equal(resolveCmsOrigin("production", " https://example.test "), "https://example.test");
});
