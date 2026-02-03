ALTER TABLE InvoiceItem 
ADD CurrencyExchangeRateId bigint(20);

update invoiceitem set CurrencyExchangeRateId=1 where Id>0;

ALTER TABLE InvoiceItem 
ADD CONSTRAINT fk_currencyexchangerate_id 
FOREIGN KEY (CurrencyExchangeRateId) 
REFERENCES CurrencyExchangeRate(id);

ALTER TABLE  `invoiceitem` 
CHANGE COLUMN `CurrencyExchangeRateId` `CurrencyExchangeRateId` BIGINT(20) NOT NULL ;