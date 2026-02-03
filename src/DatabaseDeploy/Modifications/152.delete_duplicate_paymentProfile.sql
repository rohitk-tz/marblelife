update chargecardpayment set chargecardid = 282 where chargecardid in (197,284,341,343) and isdeleted = 0;
update chargecardpayment set chargecardid = 504 where chargecardid in (449,482,484,486,488,506) and isdeleted = 0;
update chargecardpayment set chargecardid = 283 where chargecardid in (198,285,342,344) and isdeleted = 0;
update chargecardpayment set chargecardid = 505 where chargecardid in (450,483,485,487,489,507) and isdeleted = 0;

SET SQL_SAFE_UPDATES = 0;
update paymentinstrument set isdeleted = 1 where instrumententityid in (197,284,341,343,449,482,484,486,488,506,198,285,342,344,450,483,485,487,489,507);
SET SQL_SAFE_UPDATES = 1;