INSERT INTO `notificationtype` (`Id`, `Title`, `Description`) 
VALUES ('21', 'Send user Credential With Setup Guide', 'Send user Credential With Setup Guide');

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('21', '21', 'Login Credential', 'Login Credential', 'Login Credential | @Model.Franchisee', 
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 <html xmlns="http://www.w3.org/1999/xhtml">
 <head>
     <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
     <title>@Model.Base.ApplicationName</title>
     <style type="text/css">
         body {
             padding-left: 10px !important;
             padding-right: 10px !important;
             padding-top: 10px !important;
             padding-bottom: 10px !important;
             margin: 20px !important;
             width: 600px;
             border: solid 1px #B2C8D9;
             -webkit-text-size-adjust: 100% !important;
             -ms-text-size-adjust: 100% !important;
             -webkit-font-smoothing: antialiased !important;
             font-size: 16px;
         }
         a {
             color: #382F2E;
         }
 
         p, h1 {
             margin: 0;
         }
 
         p {
             text-align: left;
             font-weight: normal;
             line-height: 19px;
         }
 
         a.link1 {
             color: #382F2E;
         }
 
         a.link2 {
             text-decoration: none;
             color: #ffffff;
         }
 
         h2 {
             text-align: left;
             color: #222222;
             font-size: 19px;
             font-weight: normal;
         }
 
         div, p, ul, h1 {
             margin: 0;
         }
 
         .bgBody {
             background: #ffffff;
         }
 
         table thead tr th {
             border: 1px solid black;
             padding: 5px;
             margin: 2px;
             text-align: left;
         }
 
         table tbody tr td {
             border: 1px solid black;
             padding: 5px;
             margin: 2px;
             text-align: left;
         }
     </style>
 </head>
 <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
         <div class="contentEditableContainer contentTextEditable">
             <div class="contentEditable">
                 <span>Dear @Model.FullName,</span>
                 <br>
                 <br />
                 <p>
 				Welcome to the @Model.Base.SchedulingAppliation System.  
 				     <br />                                                 
 				Here are your Login credentials -   
 				<br>                                              
 				<br/>                                                  
 				<b>UserName :</b>  @Model.UserName  ,<br /> 
 				<b>Password :</b>  @Model.Password                                                                                                                    
 				<br/>                                              
 				<br> 
 				In order to enter and see your calendar you will visit the <a href="@Model.Base.SiteRootUrl" target="_blank" >Link</a> and enter your USER/PW combination.                                                      
 				<br> 
 				<br/>
 				<p>Once in you can see your daily calendar of estimates and jobs.
 				  You will want to take a minute and set-up any PERSONAL days you have in order to ensure that this type is set-aside whether for vacation, medical or other personal time reason.</p>   
                   <br/>				  
                   <p>Thanks for being part of the team. </p>
 				  				  <br> 
 				<br/>
 				   Regards,
                     <br /><br />
 					<p>
                    @Model.Base.OwnerName<br>
 					@Model.Base.Designation<br>
 					@Model.Base.CompanyName <br>
 					@Model.Base.Phone <br>
 					</p>
                 </p>
             </div>
         </div>
 </body>
 </html>'
);

