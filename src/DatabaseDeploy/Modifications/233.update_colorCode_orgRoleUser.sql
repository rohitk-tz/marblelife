
SET SQL_SAFE_UPDATES = 0;

UPDATE organizationroleuser AS r JOIN
        ( SELECT userid,colorcode FROM organizationroleuser 
          WHERE roleid in (3,4) 
          GROUP BY userid
        ) AS grp
       ON  grp.userid = r.userid 
SET r.colorcode = grp.colorcode WHERE r.roleid in (3,4);


SET SQL_SAFE_UPDATES = 1;