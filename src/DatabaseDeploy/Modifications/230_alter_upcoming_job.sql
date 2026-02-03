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
                      <span>Dear @Model.FullName,</span>
                      <br>
                      <br />
                      <p>
                   <p>
                   
   This is to confirm your appointment with MARBLELIFE tomorrow at @Model.StartDate at @Model.Time with Representative @Model.TechsList, pictured below.  We look forward to seeing you and assisting you with your project. Have a great day!
                 </p>
                         <br /><br />
                          @foreach (var item in Model.TechList)
                      {
                  <p>
                      @item.TechName <div class="margin-left:10px">
                      </div> <img src="@item.src" style="height: 100px; width: 100px;display:@item.display" />
                      <br/>
                  </p>
                  }
  				<br />
  				Regards,
  				
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
                     <span>Dear @Model.TechName,</span>
                     <br/>
                     <br/>
                     <p>
     				 You have been scheduled for @Model.jobTypeName with Customer @Model.FullName on @Model.StartDate at @Model.Time at @Model.Address:
 					 <p>
                      <br/>
                   The phone number for the client has been included so you can confirm you are enroute the morning of the job, and in case you are delayed enroute and need to alert the client, @Model.PhoneNumber.
 				  </p>
    				<br />
                 
 					Have great day!
                         <br/>
 						<br/>            
                         Regards,
                         <br />
                         
     					<p>
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
                     <span>Dear @Model.TechName,</span>
                     <br>
                     <br />
                     <p>
     				 This is to inform you that, your  @Model.jobTypeName  with  Customer  @Model.FullName  on  @Model.StartDate at @Model.Time has been  <b>cancelled</b>.
    				<br />
                         <br />                  
                        Have great day!
                         <br/>
                         <br/>
                         Regards,
                         <br />
                        
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
     </html>' where id=27;
     
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
                  
  Details on your @Model.jobTypeName for Customer   @Model.FullName have changed.  The @Model.jobTypeName is now scheduled for  @Model.StartDate from @Model.Time to @Model.EndTime at the following address:  @Model.Address
   <p><br/>
  The phone number for the client has been included so you can confirm you are enroute after your prior estimate that day, or you are delayed and need to alert the client.   @Model.PhoneNumber
                </p>
                <br />
  					Have great day!
                         <br/>
                         <br/>
                         Regards,
                         <br />
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
     
     update `emailtemplate` set `Body`='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                 <br />
                 <br />
                 <p>
                 We have a LAST MINTUE @Model.jobTypeName that has been added to your schedule for @Model.Time with @Model.TechName at @Model.Address .The Client number is @Model.CustomerPhoneNumber
                 <br />
                 Good Luck!
                 </p>
                  <br/>
                 Regards,
                 <br/><br/>
             </div>
             <p>
                        @Model.AssigneeName<br />
                        @Model.Base.CompanyName <br />
                        @Model.AssigneePhone <br />
             </p>
     
         </div>
     
     </body>
     </html>' where id=30;
     
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
                      
                      This is to inform you that, that your @Model.jobTypeName to Customer @Model.FullName is reassigned to you on @Model.StartDate at  @Model.Time.
                                   
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
     UPDATE `emailtemplate` SET `Subject`='@Model.jobType New: @Model.StartDate  @Model.Time, @Model.FullName, @Model.Address, @Model.FranchiseeName' WHERE `Id`='26';
     
     UPDATE `emailtemplate` SET `Subject`='@Model.jobType Re-assigned: @Model.StartDate and @Model.Time: @Model.FullName, @Model.Address: @Model.FranchiseeName' WHERE `Id`='31';
	 
	
	 
	 UPDATE `emailtemplate` SET `Subject`='@Model.jobType Cancelled: @Model.StartDate  @Model.Time, @Model.FullName, @Model.Address, @Model.FranchiseeName' WHERE `Id`='27';

UPDATE `emailtemplate` SET `Subject`='@Model.jobType Rescheduled to  @Model.StartDate  @Model.Time for @Model.FullName: @Model.FranchiseeName' WHERE `Id`='28';
UPDATE `emailtemplate` SET `Subject`='URGENT NEW @Model.jobType SCHEDULED FOR TODAY @Model.StartDate  @Model.Time with @Model.jobType at @Model.Address:  @Model.FranchiseeName' WHERE `Id`='30';


