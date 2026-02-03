ALTER TABLE `franchisedocument` 
ADD COLUMN `IsImportant` BIT(1) NOT NULL DEFAULT b'0';

ALTER TABLE `franchisedocument` 
ADD COLUMN `ShowToUser` BIT(1) NOT NULL DEFAULT b'0';

INSERT INTO `notificationtype` (`Id`, `Title`, `Description`) 
VALUES ('22', 'Document Upload Notification', 'Document Upload Notification');

INSERT INTO `notificationtype` (`Id`, `Title`, `Description`) 
VALUES ('23', 'Document Expiry Notification', 'Document Expiry Notification');

INSERT INTO `notificationtype` (`Id`, `Title`, `Description`) 
VALUES ('24', 'Document Upload Notification to Franchisee', 'Document Upload Notification to Franchisee');

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('22', '22', 'Document Upload', 'Document Upload', 'Document Upload | @Model.Franchisee', 
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
				     <b>@Model.UploadedBy</b> (<i>@Model.Email/@Model.Role</i>) of <b>@Model.Franchisee</b> has uploaded a document @Model.DocName.
                     It is marked important.				 
                     <br>
                      <br />                    
                      Kindly review it.  
                      <br>
                      <br />
                      
                      Regards,
                      <br /><br />
  					<p>
  					@Model.Base.CompanyName <br>
  					@Model.Base.Phone <br>
  					</p>
                  </p>
              </div>
          </div>
  </body>
  </html>');

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('23', '23', 'Document Expiry', 'Document Expiry', 'Document Expiry | @Model.Franchisee',
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
				     Document @Model.DocName is going to expire on @Model.ExpiryDate. Please download it if you need it.				 
                     <br>
                      <br />                    
                                           
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

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('24', '24', 'Document Upload', 'Document Upload', 'Document Upload | @Model.Franchisee', 
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
				     <b>@Model.UploadedBy</b> (<i>@Model.Email</i>) has uploaded a document @Model.DocName.
                     It is marked important.				 
                     <br>
                      <br />                    
                      Kindly review it.  
                      <br>
                      <br />
                      
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
  </html>');



