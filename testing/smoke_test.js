import { check, sleep } from 'k6';
import http from 'k6/http';

export let options = {
 	vus: 1,
	duration: '1m',
/* 	stages: [
		{ duration: '2m', target: 1000 },
		{ duration: '2m', target: 1000 },
		{ duration: '1m', target: 0 },
	  ], */
	insecureSkipTLSVerify: true,

	thresholds: {
		http_req_duration: ['p(99)<1000']
	}
};

const BASE_URL = 'https://localhost';
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

	check(loginRes, {
		'logged in successfully': (resp) => resp.json('access_token') !== ''
	});

	let authHeaders = {
		headers: {
			Authorization: `Bearer ${loginRes.json('access_token')}`,
		},
	};

	// TODO: my endpoint
	let myReservations = http.get(`${BASE_URL}/rentals/2/reservations`, authHeaders).json();

	check(myReservations, { 'retrieved reservations': (res) => res.length > 0 });

	sleep(1);
}
