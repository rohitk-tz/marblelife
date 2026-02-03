update estimateinvoiceservice
set ServiceName = 'Concrete Floor Prep - Repairs'
where ServiceName = 'Concrete Floor Prep' and Id > 0;

update estimateinvoiceservice
set ServiceName = 'Concrete Floor Prep - Grind'
where ServiceName = 'Concrete Prep Grind' and Id > 0;