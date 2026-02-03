UPDATE `notificationtype` SET `Title`='Monthly Review System Notification', `Description`='Monthly Review System Notification' WHERE `Id`='13';

UPDATE `emailtemplate` SET `Title`='Monthly Review System Notification', `Description`='Monthly Review System Notification', `Subject`='Monthly Review System Notification | @Model.Franchisee', 
`Body`='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
               padding: 5px;
               margin: 2px;
               text-align: left;
           }
   
           table tbody tr td {
               padding: 5px;
               margin: 2px;
               text-align: left;
           }
       </style>
   </head>
   <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
           <div class="contentEditableContainer contentTextEditable">
               <div class="contentEditable">
                   <font color="#454545"><span>Dear @Model.FullName,</span>
                   <br>
                   <br />
  				  <p>
				    Following list having report of total customers synced with <b>Review system</b> in the duration of <b>@Model.StartDate - @Model.EndDate</b>.
     				<br />
     				<br />
                         <table style="border: 1px solid black;border-collapse:collapse;">
                             <thead>
                                 <tr>
									 <th style="border: 1px solid black; padding: 5px;">Customer</th>
									 <th style="border: 1px solid black; padding: 5px;">Customer Email</th>
                                     <th style="border: 1px solid black; padding: 5px;">Request mode</th>
   								     <th style="border: 1px solid black; padding: 5px;">Request Date</th>
									 <th style="border: 1px solid black; padding: 5px;">Response Date</th>
									 <th style="border: 1px solid black; padding: 5px;">Response</th>
									 <th style="border: 1px solid black; padding: 5px;">Rating</th>
    							</tr>
								<thead>						
                             <tbody>
     						 @{foreach(var item in Model.ListReview) { 
                               <tr>
							         <td style="border: 1px solid black; padding: 5px;">@item.Customer</td>
                                     <td style="border: 1px solid black; padding: 5px;">@item.CustomerEmail</td>
                                     <td style="border: 1px solid black; padding: 5px;">@item.ModeOfRequest</td>
   								     <td style="border: 1px solid black; padding: 5px;">@item.RequestDate</td>
									 <td style="border: 1px solid black; padding: 5px;">@item.ResponseDate</td>
									 <td style="border: 1px solid black; padding: 5px;" width="200px">@item.Response</td>
									 <td style="border: 1px solid black; padding: 5px;">@item.Rating</td>
                                 </tr>
     						}}
							<tbody>
                         </table>  <br /><br />         				   		                      
   					</p>					 
				   Regards,
                       <br /><br />
   					<p>
                    @Model.Base.CompanyName <br>
   					@Model.Base.Phone <br>
					</font>  
               </div>
           </div>
   </body>
   </html>' WHERE `Id`='13';
