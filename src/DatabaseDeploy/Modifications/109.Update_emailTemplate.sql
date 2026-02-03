UPDATE `emailtemplate` 
SET `Subject`='Monthly - Customer Report', `Body`='<table border="0" width="600" cellpadding="20" cellspacing="0" >
  		 <tbody>
  		 <tr>
  		 <td bgcolor="#ffffff" style="border:2px solid #f2f2f2;border-collapse:collapse;">
  		 <table width="600" border="0" cellspacing="0" cellpadding="0" align="left">
  		 <tbody>
  		 <tr>
  		 <td>
  			   <font size="3" color="#454545">
  					 Dear @Model.FullName,<br /><br />
							Please find the list of serviced customers during period <b>@Model.StartDate - @Model.EndDate</b> in the attachment  below.<br /><br /><br />
							 Regards,<br /><br />
 						
  					@Model.Base.CompanyName <br>
  					@Model.Base.Phone <br>
  				</font>
  		 </td>
  		 </tr>
  		 </tbody>
  		 </table>
  		 </td>
  		 </tr>
  		 </tbody>
  		 </table>' 
WHERE `Id`='12';




UPDATE `emailtemplate` SET `Subject`='Monthly - Review System / MailChimp report', `Body`='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                   <p>Please find the Customer reports for <b>MailChimp</b> and <b>Review System</b> integration in following lists.</p><br />
				  	  
  				  <p>
				    Following list having report of total customers synced with <b>Review system</b> in the duration of <b>@Model.Startdate - @Model.EndDate</b>.
     				<br />
     				<br />
                         <table style="border: 1px solid black;border-collapse:collapse;">
                             <thead>
                                 <tr>
                                     <th style="border: 1px solid black;">Franchisee</th>
                                     <th style="border: 1px solid black;">Total Customers</th>
                                     <th style="border: 1px solid black;">Feedback Requests Send</th>
                                     <th style="border: 1px solid black;">Response Received</th>
   								  <th style="border: 1px solid black;">Total Kiosk Link Send</th>
    							</tr>
                             </thead>						
                            
     						 @{foreach(var item in Model.ListReview) { 
                             <tbody>
                                 <tr>
                                     <td style="border: 1px solid black;">@item.Franchisee</td>
                                     <td style="border: 1px solid black;">@item.TotalCustomers</td>
                                     <td style="border: 1px solid black;">@item.TotalRequests</td>
                                     <td style="border: 1px solid black;">@item.TotalResponse</td>
   								  <td style="border: 1px solid black;">@item.TotalKioskLink</td>
                                 </tr>
                             </tbody>
     						}}
                         </table>      
  				   		    				   				   
                       <br>
                       <br />                       
   					</p>
					 <p>
				   Following list having report of total customers synced with <b>MailChimp</b> in the duration of <b>@Model.Startdate - @Model.EndDate</b>.
   				<br />
   				<br />
                       <table style="border: 1px solid black;border-collapse:collapse;">
                           <thead>
                               <tr>
                                   <th style="border: 1px solid black;">Franchisee</th>
                                   <th style="border: 1px solid black;">Total Customers</th>
                                   <th style="border: 1px solid black;">Customers With Email</th>
                                   <th style="border: 1px solid black;">Synced Customers</th>
  							</tr>
                           </thead>						
                          
   						 @{foreach(var item in Model.ListEmailRecord) { 
                           <tbody>
                               <tr>
                                   <td style="border: 1px solid black;">@item.Franchisee</td>
                                   <td style="border: 1px solid black;">@item.TotalCustomers</td>
                                   <td style="border: 1px solid black;">@item.CustomersWithEmail</td>
                                   <td style="border: 1px solid black;">@item.SyncedCustomers</td>
                               </tr>
                           </tbody>
   						}}
                       </table>                    
  					<br>
                     <br />	
                   </p>
				   Regards,
                       <br /><br />
   					<p>
                    @Model.Base.CompanyName <br>
   					@Model.Base.Phone <br>
               </div>
           </div>
   </body>
   </html>' WHERE `Id`='13';
