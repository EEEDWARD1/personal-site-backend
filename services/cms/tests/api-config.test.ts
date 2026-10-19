import assert from "node:assert/strict";
import { test } from "node:test";
import { resolveApiBaseUrl } from "../lib/api-config.ts";

test("production targets the deployed API", () => {
  assert.equal(resolveApiBaseUrl("production"), "https://api.eduardteodor.co.uk/v1");
});

test("development and tests target the local API", () => {
  for (const mode of ["development", "test", undefined])
    assert.equal(resolveApiBaseUrl(mode), "http://localhost:8080/v1");
});

test("an explicit server-only override takes precedence", () => {
  for (const mode of ["production", "development"])
    assert.equal(resolveApiBaseUrl(mode, " https://another-api.example/v1 "),
      "https://another-api.example/v1");
});

test("empty overrides use the environment default", () => {
  for (const value of ["", "   "])
    assert.equal(resolveApiBaseUrl("production", value), "https://api.eduardteodor.co.uk/v1");
});
