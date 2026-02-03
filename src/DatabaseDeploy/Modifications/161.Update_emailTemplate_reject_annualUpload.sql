UPDATE `emailtemplate` 
SET `Body`='
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
				  In completing a preliminary review of your Annual-Recap-Royalty-Report submission we noted issues with the file.  A follow-up email will be forthcoming from @Model.Base.CompanyName Accounting detailing the nature of the issue,
				  so you can address and re-upload.<br>     <br>     
				  This generally means that the data format was fine – as it past the upload process – but a subsequent issue associated with the data itself was noted. 
				  You can contact @Model.Base.CompanyName Accounting for more details, if you do not have an explanatory email within a business day.
                  Thanks as always for your efforts.
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
WHERE `Id`='20';
