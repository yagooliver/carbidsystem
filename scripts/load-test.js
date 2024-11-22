import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 1000, // 10 virtual users
  duration: '30s', // Run for 30 seconds
};

export default function () {
  const url = 'http://api-gateway/bids'; // Replace with your API URL
  const payload = JSON.stringify({
    AuctionId: 1,
    Amount: Math.random() * 10000,
    UserId: `user${Math.floor(Math.random() * 10000)}`
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  const res = http.post(url, payload, params);

  check(res, {
    'status is 201': (r) => r.status === 201 || r.status === 200 || r.status === 204,
  });

  sleep(1); // Simulate wait time between requests
}