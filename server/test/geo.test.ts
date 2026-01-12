import { test } from 'node:test';
import assert from 'node:assert/strict';
import { haversineMeters } from '../src/util/geo';

test('haversineMeters computes approximate distance', () => {
  const sfLat = 37.7749;
  const sfLon = -122.4194;
  const oakLat = 37.8044;
  const oakLon = -122.2711;
  const distance = haversineMeters(sfLat, sfLon, oakLat, oakLon);
  assert.ok(distance > 10000);
  assert.ok(distance < 20000);
});
