import http from 'k6/http';
import { check, sleep, group } from 'k6';

export const options = {
  stages: [
    { duration: '30s', target: 100 },
    { duration: '30s', target: 1000 },
    { duration: '20s', target: 0 },
  ],
  thresholds: {
    http_req_duration: ['p(95)<600', 'p(90)<500'],
    http_req_failed: ['rate<0.01'],
  },
};
// export const options = {
//   vus: 1000,
//   duration: '60s',
// }

export default function () {
  group('Create a Bid', () => {
    const url = 'http://api-gateway/bids';
    const payload = JSON.stringify({
      AuctionId: 1,
      Amount: (Math.random() * 10000).toFixed(2),
      UserId: `user${Math.floor(Math.random() * 10000)}`,
    });

    const params = {
      headers: {
        'Content-Type': 'application/json',
      },
    };

    // Make the POST request
    const response = http.post(url, payload, params);

    // Perform checks
    check(response, {
      'status is 2xx': (res) => res.status >= 200 && res.status < 300, // Ensure response is successful
      'response time < 1000ms': (res) => res.timings.duration < 1000, // Validate response time
    });

    sleep(1); // Simulate user wait time
  });
}