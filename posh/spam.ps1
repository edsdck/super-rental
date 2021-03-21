for (($i = 0); $i -lt 10000; $i++)
{
	Invoke-RestMethod -Uri "http://rental.info/reservations"
	Invoke-RestMethod -Uri "http://rental.info/rentals"
}