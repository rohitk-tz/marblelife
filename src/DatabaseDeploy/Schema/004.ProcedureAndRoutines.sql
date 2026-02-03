
Drop procedure if exists createlookuptype ;
Drop procedure if exists createlookup;

delimiter //
Create procedure createlookuptype (p_id bigint, p_label varchar(128), p_alias varchar(128)) 
begin 	
	insert into LookupType (Id, `Name`, Alias) values (p_id, p_label, p_alias);
end//
delimiter ;

delimiter //
Create procedure createlookup (p_id bigint, p_lookuptypeid bigint, p_name varchar(512), p_alias varchar(512)) 
begin 

	Select  Max(RelativeOrder) into @relOrder from Lookup where LookupTypeId = p_lookuptypeid;

	if (@relOrder is null) then
		Set @relOrder = 0;
	end if;

	Set @relOrder = @relOrder + 1;

	insert into Lookup (Id, LookupTypeId, `Name`, Alias, RelativeOrder)
	values (p_id, p_lookuptypeid, p_name, p_alias, @relOrder);

end//
delimiter ;