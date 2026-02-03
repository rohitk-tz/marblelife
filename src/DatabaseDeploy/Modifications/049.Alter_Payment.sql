ALTER TABLE Payment 
ADD CurrencyExchangeRateId bigint(20);


update Payment set CurrencyExchangeRateId=1 where Id>0;


ALTER TABLE Payment 
ADD CONSTRAINT fk_exchangerate_id 
FOREIGN KEY (CurrencyExchangeRateId) 
REFERENCES CurrencyExchangeRate(id);


ALTER TABLE  `Payment` 
CHANGE COLUMN `CurrencyExchangeRateId` `CurrencyExchangeRateId` BIGINT(20) NOT NULL ;