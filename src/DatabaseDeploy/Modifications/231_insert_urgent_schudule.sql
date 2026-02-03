INSERT INTO `NotificationType` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) VALUES ('32', 'Urgent Job Change', 'Urgent Job Change', 1, 1, 0);

INSERT INTO `EmailTemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`, `IsDeleted`) VALUES ('32', '32', 'Urgent Job Change', 'Urgent Job Change', 'URGENT NEW @Model.jobType SCHEDULED FOR TODAY @Model.StartDate  @Model.Time with @Model.jobType at @Model.Address:  @Model.FranchiseeName', 
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
                  <br />
                  <br />
                  <p>
                  We have a LAST MINTUE @Model.jobTypeName that has been added to your schedule on @Model.StartDate for @Model.Time with @Model.TechName at @Model.Address .The Client number is @Model.CustomerPhoneNumber
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
      </html>'
, 0);

UPDATE `emailtemplate` SET `Subject`='URGENT NEW @Model.jobType SCHEDULED FOR @Model.dateType  @Model.StartDate  @Model.Time with @Model.jobType at @Model.Address:  @Model.FranchiseeName' WHERE `Id`='32';
UPDATE `emailtemplate` SET `Body`='<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n       <html xmlns=\"http://www.w3.org/1999/xhtml\">\n       <head>\n           <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n           <title>@Model.Base.ApplicationName</title>\n           <style type=\"text/css\">\n               body {\n                   padding-left: 10px !important;\n                   padding-right: 10px !important;\n                   padding-top: 10px !important;\n                   padding-bottom: 10px !important;\n                   margin: 20px !important;\n                   width: 600px;\n                   border: solid 1px #B2C8D9;\n                   -webkit-text-size-adjust: 100% !important;\n                   -ms-text-size-adjust: 100% !important;\n                   -webkit-font-smoothing: antialiased !important;\n                   font-size: 16px;\n               }\n       \n               a {\n                   color: #382F2E;\n               }\n       \n               p, h1 {\n                   margin: 0;\n               }\n       \n               p {\n                   text-align: left;\n                   font-weight: normal;\n                   line-height: 19px;\n               }\n       \n               a.link1 {\n                   color: #382F2E;\n               }\n       \n               a.link2 {\n                   text-decoration: none;\n                   color: #ffffff;\n               }\n       \n               h2 {\n                   text-align: left;\n                   color: #222222;\n                   font-size: 19px;\n                   font-weight: normal;\n               }\n       \n               div, p, ul, h1 {\n                   margin: 0;\n               }\n       \n               .bgBody {\n                   background: #ffffff;\n               }\n       \n               table thead tr th {\n                   border: 1px solid black;\n                   padding: 5px;\n                   margin: 2px;\n                   text-align: left;\n               }\n       \n               table tbody tr td {\n                   border: 1px solid black;\n                   padding: 5px;\n                   margin: 2px;\n                   text-align: left;\n               }\n           </style>\n       </head>\n       <body style=\"background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;\">\n           <div class=\"contentEditableContainer contentTextEditable\">\n               <div class=\"contentEditable\">\n                   <span>Dear @Model.FullName,</span>\n                   <br />\n                   <br />\n                   <p>\n                   We have a LAST MINTUE @Model.jobTypeName that has been added to your schedule for @Model.dateType on @Model.StartDate for @Model.Time with @Model.TechName at @Model.Address .The Client number is @Model.CustomerPhoneNumber\n                   <br />\n                   Good Luck!\n                   </p>\n                    <br/>\n                   Regards,\n                   <br/><br/>\n               </div>\n               <p>\n                          @Model.AssigneeName<br />\n                          @Model.Base.CompanyName <br />\n                          @Model.AssigneePhone <br />\n               </p>\n       \n           </div>\n       \n       </body>\n       </html>' WHERE `Id`='32';





