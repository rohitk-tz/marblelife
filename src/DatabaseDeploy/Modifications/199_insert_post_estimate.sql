insert into `notificationtype` (`Id`, `Title`, `Description`) 
values('29','Service Deposit','Service Deposit');

insert into emailtemplate (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`)
 values('29','29','Service Deposit','Service Deposit','Service Deposit| @Model.Franchisee',
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    
    <style type="text/css">
	    tr td a
     	     {
	          text-decoration:none;
			  
			}
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
        .text_center td p {
		text-align:center;
        }
		
		.text_center td .big {
		font-size:20px;
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
            <br>
            <br />
            <p>
               <table>
                   <tr class=text_center><td><p class="big"><b>Thank You for Considering MARBLELIFE</b></p></td></tr>
				   <tr><td>Your feedback and opinion matter to us.  In fact, it is clients such as yourself that have driven the direction and development of MARBLELIFE since 1980, to become the largest stone 
				   restoration company in North America.</td></tr>
				   <tr class=text_center><td><p>Would you mind reviewing your assessment visit?</p>
                              <p>Your input helps us recognize superior team performance.</p>
                    </td></tr>
					   <tr class=text_center><td><p><b>DID YOU KNOW?</b></p>
                              <p style="color:red">20% of MARBLELIFE’s Restoration Work .</p>
							  <p style="color:red">is the result of use of an inappropriate cleaner?</p>
                      </td></tr>
					  <tr><td>
							Studying how surfaces wear and get dirty, and why people needed our services, allowed MARBLELIFE to understand and develop a line of care products very different from those found on the grocery shelf. 
							Our products work without creating damage.  In fact, they work so well our teams use them exclusively.  
                      </td></tr>
					  <tr><td>
							If they were not the best we would be researching those that were in order to improve them.  As we, like you, want to get the job done quickly, effectively, without creating damage or additional downstream work.   
                      </td></tr>
					  <tr><td>
							<a href="#">Click here </a> to see MARBLELIFE’s care recommendations for your just completed project.  We will save you future time, money and anguish.  Let’s keep your property looking its best.    
                      </td></tr>
					  <tr><td>
							MARBLELIFE looks forward to assisting you in caring for your hard surfaces.    
                      </td></tr>
					  <tr class=text_center><td><p class="big">
							<b>APPRECIATION COUPON:                           10% OFF</p>
                             <p class="big">Keep Your Surfaces Sparkling!</b></p>
                      </td></tr>
					  <tr><td>
							<p><b>We appreciate all of our MARBLELIFE customers.
							Receive 10% OFF any product purchase with coupon code: service10off</p>
                            <p>*Valid until: 30 Days From Today</b></p>
                      </td></tr>
               </table>

               </div>
        </div>
</body>
</html>'
);