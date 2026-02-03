update estimateinvoiceservice
set StoneType = 'Satin/Semi-Gloss'
Where StoneType = 'Satin' and Id>0;

update estimateinvoiceservice
set StoneType = 'Satin/Semi-Gloss'
Where StoneType = 'Semi-Gloss' and Id>0;

update beforeafterImages
set FinishMaterial = 'Satin/Semi-Gloss'
Where FinishMaterial = 'Semi-Gloss' and Id>0;

update beforeafterImages
set FinishMaterial = 'Satin/Semi-Gloss'
Where FinishMaterial = 'Satin' and Id>0;

update jobestimateservices
set FinishMaterial = 'Satin/Semi-Gloss'
Where FinishMaterial = 'Semi-Gloss' and Id>0;

update jobestimateservices
set FinishMaterial = 'Satin/Semi-Gloss'
Where FinishMaterial = 'Satin' and Id>0;