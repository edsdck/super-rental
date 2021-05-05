import { check, sleep } from 'k6';
import http from 'k6/http';

export let options = {
 	vus: 50,
	duration: '1m',
	insecureSkipTLSVerify: true,
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

	let authHeaders = {
		headers: {
			Authorization: `Bearer ${loginRes.json('access_token')}`,
		},
	};

	let myReservations = http.get(`${BASE_URL}/rentals/2/reservations`, authHeaders).json();

	sleep(1);
}
