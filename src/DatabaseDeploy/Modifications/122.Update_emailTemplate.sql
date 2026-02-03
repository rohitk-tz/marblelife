INSERT INTO `notificationtype` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) 
VALUES ('15', 'Monthly Sales Upload Notification', 'Monthly Sales Upload Notification', 1, 1, 0);

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('15', '15', 'Monthly Sales Upload Notification', 'Monthly Sales Upload Notification', 'Monthly Sales Upload', 
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
                  <span>Dear @Model.Base.OwnerName,</span>
                  <br>
                  <br />
                  <p>
 				@if(Model.List.Count() <= 0)
 				{
 					  <span>None of the Franchisee(s) have missed the upload for the duration<b>@Model.StartDate</b> to <b>@Model.EndDate</b>.</span>
 					  <br /><br />
 				}
  				@if(Model.List.Count() > 0)
 				{
 					  <span>Following is the list of Franchisee(s) whose uploads are not on time during <b>@Model.StartDate</b> to <b>@Model.EndDate</b>.</span>
 						<br /><br />
                      <table style="border: 1px solid black;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Franchisee</th>
                                  <th style="border: 1px solid black;">Payment Frequency</th>
								  <th style="border: 1px solid black;">Late Upload Details</th>								  
                             </tr>
                          </thead>
  						 @foreach(var item in Model.List) 
 						 { 
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@item.Franchisee</td>
                                  <td style="border: 1px solid black;">@item.FeeProfile</td>								  
								  <td>
								  
								  <table style="border: 1px solid black;">
										   <thead>
												<tr>
												  <th style="border: 1px solid black;">Start Date</th>
												  <th style="border: 1px solid black;">End Date</th>
												  <th style="border: 1px solid black;">Uploaded</th>	
												  <th style="border: 1px solid black;">Uploaded On</th>	
												 <th style="border: 1px solid black;">Expected Upload Date</th>													  
												</tr>
											</thead>
											@foreach(var record in item.RecordCollection) 
											 { 
											  <tbody>
												  <tr>
													  <td style="border: 1px solid black;">@record.StartDateString</td>
													  <td style="border: 1px solid black;">@record.EndDateString</td>
													  <td style="border: 1px solid black;">@record.Uploaded</td>
													  <td style="border: 1px solid black;">@record.UploadedOnString</td>
													  <td style="border: 1px solid black;">@record.ExpectedUploadDateString</td>
													</tr>
											  </tbody>
											}
								   </table> 		
								  
								  </td>						   
 							  </tr>
                          </tbody>
  						}
                      </table> 
                      <br />
                      <br /> 
				 }			 				
                      Regards,
                      <br /><br />
  					<p>                    
  					@Model.Base.CompanyName <br>
  					</p>
                  </p>
              </div>
          </div>
  </body>
  </html>');

