update emailtemplate
Set Body = '<!DOCTYPE html
      PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
  <html xmlns="http://www.w3.org/1999/xhtml">
  <head>
      <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
      <title>@Model.Base.CompanyName</title>
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
          p {
              text-align: left;
              font-weight: normal;
              line-height: 19px;
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
          .bgBody {
              background: #ffffff;
          }
          .block {
              display: block;
          }
          table,
          th,
          td {
              border: 1px solid black;
              border-collapse: collapse;
              padding: 3px;
          }
          .none {
              display: none;
          }
      </style>
  </head>
  <body
      style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
      <div class="contentEditableContainer contentTextEditable">
          <div class="contentEditable">
              <p>MARBLELIFE ROYALTY REPORTING</p><br>
              <p>Dear Admin,<br>
                  This is to notify you that the Franchisee(s) has exceeded the Bulk Corporate Price by 50%.
                  <br><br>Please find the details below of the changes.
              </p>
              <table>
                  <thead>
                      <tr>
                          <td>Franchisee</td>
                          <td>List of Services</td>
                          <td>Max Price</td>
                          <td>Corporate  Price</td>
                          <td>Price Type <br>(A)rea, (E)vent, (L)inear Ft, (P)roduct Price, (T)ax Rate, (T)ime</td>
                          <td>How this is calculated</td>
                      </tr>
                  </thead>
                  <tbody>
                      @foreach (var item in Model.EmailNotificationOnFranchiseePriceExceedViewModels)
                      {
                         <tr>
                             <td rowspan=@item.Count>@item.FranchiseeName</td>
                             @foreach (var itemServices in item.ListOfServicesForBulkUploadPrices)
                             {
                                <tr>
                                    <td>@itemServices.ListOfServicesName</td>
                                    <td>@itemServices.Price</td>
                                    <td>@itemServices.CoporatePrice</td>
                                    <td>@itemServices.PriceType</td>
                                    <td>@itemServices.HowCalculated</td>
                                </tr>
                             }
                         </tr>
                      }
                  </tbody>
              </table>
              <br>
              <div>
                  <p>Have a great day!<br>Regards,
                  </p>
              </div>
              <br>
              <p>MARBLELIFE ®</p>
          </div>
      </div>
  </body>
  </html>'
 where (Id = 578 and NotificationTypeId = 58);