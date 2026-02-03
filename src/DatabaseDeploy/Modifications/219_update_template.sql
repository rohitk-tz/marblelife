update `emailtemplate` set `Body`='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
   <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.ApplicationName</title><style type="text/css">body {
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
                   <span>Dear @Model.TechName</span>
                   <br>
                   <br />
                   <p>
   				 You have been scheduled for a  @Model.jobTypeName  to  Customer  @Model.FullName  on  @Model.StartDate at @Model.Time at  @Model.Address.
                 The phone number for the client has been included so you can confirm you are enroute the morning of the job, and in case you are delayed enroute and need to alert the client. @Model.PhoneNumber
  				<br />
					Have great day!
                       <br>
                       <br />                  
                       Regards,
                       <br /><br />
   					<p>
                   <br />
                     @Model.AssigneeName<br />
                     @Model.Base.CompanyName <br />
                     @Model.AssigneePhone <br />
   					</p>
                   </p>
               </div>
           </div>
   </body>
   </html>' where id=26;
   
   update `emailtemplate` set `Body`='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
   <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.ApplicationName</title><style type="text/css">body {
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
                   <span>Dear @Model.TechName</span>
                   <br>
                   <br />
                   <p>
                Details on your JOB for Customer @Model.FullName have changed.  The job is now scheduled for @Model.StartDate from  @Model.Time to  @Model.EndTime at the following address:  @Model.Address
				The phone number for the client has been included so you can confirm you are enroute the morning of the job, and in case you are delayed enroute and need to alert the client.   @Model.PhoneNumber
                
                <br />
					Have great day!
                       <br>
                       <br />                  
                       Regards,
                       <br /><br />
   					<p>
                   <br />
                     @Model.AssigneeName<br />
                     @Model.Base.CompanyName <br />
                     @Model.AssigneePhone <br />
   					</p>
                   </p>
               </div>
           </div>
   </body>
   </html>' where id=31;
   
   update `emailtemplate` set `Body`='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
   <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.ApplicationName</title><style type="text/css">body {
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
                   <span>Dear @Model.TechName,</span>
                   <br>
                   <br />
                   <p>
                <p>
                Dear @Model.FullName
 
This is to confirm your appointment with MARBLELIFE tomorrow at @Model.StartDate with MARBLELIFE Representative NAME, pictured below.  We look forward to seeing you and assisting you with your project.  Have a great day!
              </p>
              <br />
					Have great day!
                       <br>
                       <br />                  
                       Regards,
                       <br /><br />
                       @foreach (var item in Model.TechList)
                   {
               <p>
                   @item.TechName <div class="margin-left:10px">
                   </div> <img src="@item.src" style="height: 100px; width: 100px;" />
                   <br/>
               </p>
               }
   					<p>
                   <br />
                     @Model.AssigneeName<br />
                     @Model.Base.CompanyName <br />
                     @Model.AssigneePhone <br />
   					</p>
                   </p>
               </div>
           </div>
   </body>
   </html>' where id=25;
   
   
   update `emailtemplate` set `Body`='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
   <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.ApplicationName</title><style type="text/css">body {
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
                   <span>Dear @Model.TechName</span>
                   <br>
                   <br />
                   <p>
                Dear @Model.TechName
Details on your @Model.jobTypeName for Customer   @Model.FullName have changed.  The @Model.jobTypeName is now scheduled for  @Model.StartDate from @Model.Time to @Model.EndTime at the following address:  @Model.Address
 
The phone number for the client has been included so you can confirm you are enroute after your prior estimate that day, or you are delayed and need to alert the client.   @Model.PhoneNumber
                <br />
					Have great day!
                       <br>
                       <br />                  
                       Regards,
                       <br /><br />
   					<p>
                   <br />
                     @Model.AssigneeName<br />
                     @Model.Base.CompanyName <br />
                     @Model.AssigneePhone <br />
   					</p>
                   </p>
               </div>
           </div>
   </body>
   </html>' where id=28;
   
update `emailtemplate` set `Subject`='@Model.jobTypeName New: @Model.StartDate  @Model.Time, @Model.FullName, @Model.Address, @Model.FranchiseeName' where id=26;

update `emailtemplate` set `Subject`='@Model.jobTypeName Cancelled: @Model.StartDate  @Model.Time, @Model.FullName, @Model.Address, @Model.FranchiseeName' where id=27;

update `emailtemplate` set `Subject`='@Model.jobTypeName Rescheduled to  @Model.StartDate  @Model.Time for @Model.FullName: @Model.FranchiseeName' where id=28;

update `emailtemplate` set `Subject`='Today @Model.jobTypeName Scheduled: @Model.StartDate  @Model.Time, @Model.FullName,@Model.Address, @Model.FranchiseeName' where id=30;

UPDATE `makalu`.`emailtemplate` SET `Title`='ReAssigned JobEstimate', `Description`='ReAssigned JobEstimate' WHERE `Id`='31';
