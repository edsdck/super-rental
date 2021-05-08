import { check, sleep } from 'k6';
import http from 'k6/http';

export let options = {
    stages: [
        { duration: '5m', target: 250 },
        { duration: '10m', target: 250 },
        { duration: '5m', target: 0 },
    ],
    insecureSkipTLSVerify: true,

    thresholds: {
        http_req_duration: ['p(99)<5000'],
        http_req_duration: ['p(50)<1000'],
        http_req_failed: ['rate<0.01'],
        'checks{myTag:loginSuccess}': ['rate>0.95']
    }
};

const BASE_URL = 'http://localhost/api';
const USERNAME = 'bob';
const PASSWORD = 'bob';

export default function () {
    let formData = {
        username: USERNAME,
        password: PASSWORD,
        client_id: 'client',
        client_secret: 'secret',
        scope: 'api1 openid profile',
        grant_type: 'password'
    };
    let headers = { 'Content-Type': 'application/x-www-form-urlencoded' };

    let loginRes = http.post(`${BASE_URL}/connect/token`, formData, { headers: headers });

    check(loginRes,
        { 'logged in successfully': (resp) => resp.json('access_token') !== '' },
        { myTag: 'loginSuccess' }
    );

    let authHeaders = {
        headers: {
            Authorization: `Bearer ${loginRes.json('access_token')}`,
        },
    };

    let myRentals = http.get(`${BASE_URL}/rentals/me`, authHeaders).json();

    check(myRentals, { 'retrieved rentals': (resp) => resp.length > 0 });

    let rentalReservations = http.get(`${BASE_URL}/rentals/4/reservations`, authHeaders).json();

    check(rentalReservations, { 'retrieved reservations': (resp) => resp.length > 0 });

    sleep(1);
}
