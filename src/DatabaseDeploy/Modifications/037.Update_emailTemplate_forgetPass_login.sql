UPDATE `EmailTemplate` SET `Body`=
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
 <br> <br/>                        
 <p>                                                        
 We were asked to reset your @Model.Base.CompanyName account password.                                                        
 <br>                                                        
 <br/>                                                        
 Click the following link to set a new password. Please note, this link will expire in 24 hours.
 <br>                                                        
 <br/>                                                        
 <a href="@Model.PasswordLink" target=\"_blank\">ResetLink</a>                                                        
 <br>
 <br/>                                                  
 Thank you,                                                        
 <br>                                                        
 <br>                                                        
 @Model.Base.CompanyName                                                  
 </p>                                                     
</p>                                                
</div>                                            
</div>  
</body></html>'
 WHERE `Id`='1';

 UPDATE `EmailTemplate` SET `Body`=
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
 <p >                                                        
 We’d like to be the first to welcome you to @Model.Base.CompanyName.                                                      
 <br>                                                        
 <br/>                                                        
 Here are your Login credentials -   
 <br>                                              
 <br/>                                                  
 <b>UserName :</b>  @Model.UserName  ,<br /> 
 <b>Password :</b>  @Model.Password                                                                                                                    
 <br/>                                              
 <br> 
 You can <a href="@Model.Base.SiteRootUrl" target="_blank" >Login Now.</a>                                                      
 <br> 
 <br/>                                                       
 Thank you,    
 <br>
 <br/>                                                       
 @Model.Base.CompanyName                                                  
</p>                                                
</div>                                            
</div>  
</body></html>'
 WHERE `Id`='2';
