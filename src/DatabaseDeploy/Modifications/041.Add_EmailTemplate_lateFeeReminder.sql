INSERT INTO `NotificationType` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) VALUES ('6', 'LateFee Reminder', 'LateFee Reminder', 1, 1, 0);

INSERT INTO `EmailTemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`, `IsDeleted`) VALUES ('6', '6', 'LateFee Reminder', 'LateFee Reminder', 'LateFee Reminder',
'<!DOCTYPE html PUBLIC \\\"-//W3C//DTD XHTML 1.0 Transitional//EN\\\" \\\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\\\">
<html xmlns=\\\"http://www.w3.org/1999/xhtml\\\"><head>    <meta http-equiv=\\\"Content-Type\\\" content=\\\"text/html; charset=utf-8\\\" />    
<title>Reset password | Maestro</title>    
<style type=\\\"text/css\\\">      
body {       
padding-top: 0 !important;
padding-bottom: 0 !important;
padding-top: 0 !important;       
padding-bottom: 0 !important;       
margin:0 !important;       
width: 100% !important;       
-webkit-text-size-adjust: 100% !important;       
-ms-text-size-adjust: 100% !important;       
-webkit-font-smoothing: antialiased !important;     
}     .
tableContent img {       
border: 0 !important;       
display: block !important;       
outline: none !important;     
}     
a{      
color:#382F2E;    
}    
p, h1{      
color:#382F2E;      
margin:0;    
}    
p{      
text-align:left;      
color:#999999;      
font-size:14px;      
font-weight:normal;      
line-height:19px;    
}    
a.link1{      
color:#382F2E;    
}    
a.link2{      
font-size:16px;      
text-decoration:none;      
color:#ffffff;    
}    
h2{      
text-align:left;       
color:#222222;        
font-size:19px;      
font-weight:normal;    
}    
div,p,ul,h1{
margin:0;    
}    
.bgBody{      
background: #ffffff;    
}    
.bgItem{      
background: #ffffff;    
}    
</style></head>
<body paddingwidth=\\\"0\\\" paddingheight=\\\"0\\\"   style=\\\"padding-top: 0; padding-bottom: 0; padding-top: 0; padding-bottom: 0; background-repeat: repeat; width: 100% !important; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;\\\" offset=\\\"0\\\" toppadding=\\\"0\\\" leftpadding=\\\"0\\\"><table width=\\\"100%\\\" border=\\\"0\\\" cellspacing=\\\"0\\\" cellpadding=\\\"0\\\" class=\\\"tableContent bgBody\\\" align=\\\"center\\\"  style=\\\\\'font-family:Helvetica, Arial,serif;\\\\\'>
<div class=\\\"contentEditableContainer contentTextEditable\\\">                                                
<div class=\\\"contentEditable\\\" align=\\\\\'center\\\\\'>                            
<span>Hello @Model.FullName,</span> 
<br>
<br/>                        
<p>  
Late fee has been charged for Invoice# - @Model.InvoiceId for <b>@Model.Franchisee</b>. 
please make payment to avoid getting charged with interest rate.                                        
<br>                                                        
<br/>  
<span>Invoice Amount -  @Model.Amount</span>                                                       
<br>
<br/> 
<span>LateFee For -  @Model.LateFeeType</span>
<br>
<br/>
<span>Late Fee -  @Model.LateFee</span>
 <br>                                                        
 <br/>                                                       
<span>Invoice# -  @Model.InvoiceId</span>
 <br>                                                        
 <br/>                                                  
Thank you,                                                        
<br>                                                        
<br>                                                        
@Model.Base.CompanyName                                                  
</p>                                                
</div>                                            
</div>  
</body></html>'
 , 0);
