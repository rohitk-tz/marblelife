CREATE TABLE `termsAndCondition` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `TermAndCondition` text NOT NULL,
   `TyepeId` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
 KEY `fk_termsAndCondition_lookup1_idx` (`TyepeId`),
  CONSTRAINT `fk_termsAndCondition_lookup1` FOREIGN KEY (`TyepeId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
);

INSERT INTO `termsAndCondition` ( `TyepeId`,`TermAndCondition`,`IsDeleted`) VALUES ('240','<table style="width:100%;" cellpadding="0" cellspacing="0">
               
                <tbody>
                    <tr>
                        <td style="vertical-align:top;max-width:50%;width:50%;min-width:50%">
                            <div>
                                <span>Proper environmental control must be provided.</span>
                                <br />
                                <ul>
                                    <li>Owner/Buyer to provide clean water and permanent HVAC during installation.</li>
                                    <li><u>Temperature control must be maintained at or above 60 degrees F before and during installation.</u></li>
                                    <li>Owner/Buyer must supply onsite trash receptacles sufficient for empty 5 gallon buckets and job site debris.</li>
                                </ul>

                                <p>
                                    <b> Lighting -</b> Failure to provide proper lighting, electricity and/or environmental control may result in back charges.
                                </p>
                                <p>
                                    <b>Scheduling -</b> At no time shall the speed of the project completion be allowed to detrimentally affect the application or impede the system tolerances.
                                </p>
                                <p>
                                    <b>Moisture Levels -</b> This proposal assumes moisture levels acceptable for the installation of the system
                                    (3-5# per 1000 sf or 75-80% RH within the substrate - or as recommended by the manufacturer). The concrete surface must be inspected
                                    and quantitative vapor measurements taken prior to the execution of vapor emission compliance procedures. MARBLELIFE will not be
                                    responsible for moisture related problems. Owner/Buyer is responsible for all tests prior to our arrival.
                                </p>
                                <b>Ventilation -</b> MARBLELIFE and Owner/Buyer shall ensure that proper ventilation systems are in place prior to installation.
                                <p>
                                    <b>Floor Protection - </b>ProGuard protection is always recommended for protection of the floor before and after installation.
                                    During and/or after completion, any damage to the installed system, not caused by MARBLELIFE employees, will result in additional
                                    charges for repairs. If the finished floors are not protected and damage results, additional charges for repair will be added as a
                                    change order to the original contract.
                                </p>
                                <p>
                                    <b>Substrate Condition -</b> Substrate must be structurally sound and in stable condition. MARBLELIFE will not be responsible for sub grade
                                    shifting, cracking, defects that may occur during freeze/thaw or any other ground movements, or improper curing methods resulting in
                                    discoloration of the substrate. This proposal is contingent upon the concrete substrate(s) meeting minimum SBC standards and ACI 318
                                    Building Code for Structural Concrete. Deviations from the SBC or ACI 318 not disclosed in writing prior to installation will result
                                    in any warranties becoming void upon discovery.
                                </p>
                                <p>
                                    <b>Acidity -</b> This proposal assumes pH levels acceptable for the installation of this system (no more than 6-7 pH or as recommended by
                                    the manufacturer). The concrete must be inspected and quantitative pH measurements taken prior to the execution of dye compliance
                                    procedures. MARBLELIFE will not be responsible for pH related problems.
                                </p>
                                <p>
                                    <b> Clean Work Area -</b> Work area for installation must be scraped, broom cleaned and free from all debris including but not limited to,
                                    drywall dust and residue, paint, previous coatings, glues, adhesives, chalk or snap lines, etc. The Owner/Buyer shall pay expenses
                                    of any delay or additional cost caused by the failure of the Owner/Buyer to provide working conditions described herein.
                                </p>
                                <p>
                                    <b> Floor Removal - </b>This proposal excludes the removal of all existing floor covering materials, unless otherwise noted.
                                </p>
                                <p>
                                    <b>Base Molding -</b> Base molding shall not be installed prior to installation/floor polishing.It is the Owner/Buyer responsibility to
                                    accept the level of variations, blemishes, etc. on the surface of the substrate prior to the application of the polishing, coloration, and sealant systems.
                                </p>
                                <p>
                                    <b>Concrete Substrate Protection -</b> The concrete substrate must be protected during construction according to The
                                    Concrete Surface Finish Specification. During the initial preparation stage, MARBLELIFE will attempt to remove
                                    minor unexpected markings, but will not guarantee complete removal of stains. All parties should be aware that
                                    many markings and penetrated stains including but not limited to footprints, paint, primer, construction markings,
                                    blemishes, glues, caulking, and patches in the surface may reappear after stain is applied, and may reappear in the
                                    finished floor.
                                </p>
                                <p>
                                    Do not use muriatic acid, Pine-Sol, citric cleaners, solvents, or chemicals on bare concrete as it may hinder the application of the
                                    stains/dyes.
                                </p>
                                <p>
                                    Ensuring that these requirements are met will guarantee the best possible installation, completed on time and within budget.
                                    We appreciate your assistance.
                                </p>
                                <p><div><b>MARBLELIFE:</b><div style="width:30%;border-bottom:1px solid black;margin-left:20% !important"></div></div></p>
                                <p><div><b>Owner/Buyer:</b><div style="width:30%;border-bottom:1px solid black;margin-left:20% !important"></div></div></p>
                                <p><b>Conditions of Installation:</b></p>
                                <p>
                                    <b>Stain Variations -</b> Polished Concrete Systems are hand crafted onsite by trained applicators using a variety of
                                    techniques and artisan craftsmanship. They are not pre-manufactured flooring systems and therefore every system
                                    installation is unique and will present its own set of installation variables. Similar in effect to natural stone
                                    products, the final outcome of these systems are designed to and may give an assortment of colors, swirls, mottling
                                    effects, blemishes, and cracks natural to and desired when choosing these types of handcrafted systems.
                                </p>
                                <p>
                                    <b>Surface Blemishes -</b> Our Polished Concrete Systems create custom translucent color effects and are not designed to
                                    hide surface discoloration, blemishes, impressions, cracks, markings or other construction variables.
                                </p>
                                <p>
                                    Our Decorative & Polished Concrete Surfaces are striking in appearance and add to the artistic features of modern
                                    sculptures, floors, walls, and accessories.
                                </p>
                                <br />
                            </div>
                        </td>
                        <td style="vertical-align:top;max-width:50%;width:50%;min-width:50%">
                            <div>
                                <p>
                                    <b> Maintenance -</b> Maintenance of these systems is recommended and instructions will be provided. Maintenance is the sole
                                    responsibility of the end user, unless otherwise agreed to with a maintenance contract.
                                </p>
                                <p>
                                    <b> Security -</b> Upon delivery to the job site, the Owner/Buyer shall protect all materials and equipment from damage,
                                    theft, or abuse.
                                </p>
                                <p>
                                    <b> Literature -</b> Please ensure that all parties involved review the product literature and information submitted by
                                    MARBLELIFE before work begins.
                                </p>
                                <p>
                                    <b>Deposit Required -</b> No material will be ordered, nor is manpower scheduled, until a signed copy of this
                                    proposal, along with the required deposit, is returned to MARBLELIFE
                                </p>
                                <p>
                                    <b> Adjacent Surfaces -</b> MARBLELIFE employees will do their best to protect adjacent surfaces and provide dust control.
                                    However, due to the nature of the process and the mass of the equipment we are not responsible for finished surfaces
                                    (drywall, painted surfaces, doors, baseboards, cabinetry, etc.), or dust. All furniture and fixtures are the
                                    responsibility of the Owner/Buyer.
                                </p>
                                <p>
                                    <b>Edge Work -</b> Edges to be finished within 1/2'' of wall. Finishing edges closer will be at an additional cost.
                                </p>
                                <p>
                                    Due to the difference in resin tooling and equipment, edges may have a slight variation in color and sheen.
                                </p>
                                <p>
                                    <b>Concrete Hardness -</b> This proposal assumes that the concrete substrate is of medium strength. If the concrete is soft,
                                    hard or has a burnished finish that increases the intended/estimated wear of the diamond tooling, MARBLELIFE reserves
                                    the right to increase the price to cover the additional labor and material cost.
                                </p>
                                <p>
                                    <b> Unencumbered Access - </b>Owner/Buyer to provide unencumbered access to the job site and work area, and we will be
                                    allowed parking for the equipment trailer to within 15 ft of the building.
                                </p>
                                <p>
                                    <b>Sample - </b>One custom 7x7 mock-up /sample will be created using the specified system. The mock up/sample area
                                    provided by Owner/Buyer must be of the same concrete mix design, installation and finish methods as specified for
                                    the project.
                                </p>
                                <p>
                                    The Owner/Buyer agrees to pay $2500.00 for the sample (amount to be credited toward purchase upon notice to proceed),
                                    unless otherwise waived in writing.
                                </p>
                                <p>
                                    Owner/Buyer agrees to pay time/material for additional mock-up /samples that may be required. Owner/Buyer
                                    (or representative) must provide signed approval for mock-up/sample before work-begins.
                                </p>

                                <p><b><u>Special Instructions:</u></b></p>
                                <p>
                                    <b> Concrete Surface Protection -</b> Please ensure that the concrete substrate is properly cured, protected from markings,
                                    damage, paint and drywall, caulking residue, blemishes and all construction debris.
                                </p>
                                <p>
                                    MARBLELIFE will not be responsible for imperfections in the substrate that show through the colors and finishes.
                                </p>
                                <p>
                                    Do not allow permanent marker, pencil, red/blue/black chalk lines on flooring to receive transparent color and
                                    clear finishes.provided. Variables that occur during installation are due to each unique working environment (i.e. lighting,
                                    temperature, airflow, humidity, and existing floor conditions)
                                </p>
                                <p>
                                    <b> and Acceptance -</b> By entering into this agreement and choosing our Decorative & Polished Concrete System,
                                    the end user understands and is agreeing to accept the look and variables expected when choosing this type of system,
                                    and have taken the time to educate themselves on the artistic and unique impression that these systems portray
                                </p>
                                <p>
                                    The total system design must be considered to ensure safe, long lasting, and trouble free performance. MARBLELIFE
                                    does not take the responsibility to determine and/or specify systems chosen. Function, material compatibility,
                                    adequate physical characteristics, and maintenance, are the responsibility of the Architect, Designer, General
                                    Contractor and ultimately the end user. It is the end user responsibility to determine the suitability of these
                                    systems for their particular application and their own use.
                                </p>
                                <p><b>Slip and Fall Precautions:</b></p>
                                <p>
                                    <b> Maintenance - </b>MARBLELIFE and the Manufacturer of these Decorative & Polished Concrete Systems recommend the use of
                                    proper cleaning and maintenance as well as the use of proper cleaning products for polished concrete flooring systems.

                                </p>
                                <p>
                                    Immediate attention to areas that may be exposed to wet, oily or greasy conditions are highly recommended. These
                                    products are designed specifically for polished concrete, leaving a low residue build up to aid in a higher degree
                                    of slip resistance.
                                </p>
                                <p>
                                    Polished concrete surfaces can be slippery when improperly cleaned and maintained. The type of activity on the
                                    flooring surface, maintenance procedures, cleaning products, cleaning residue, and type of footwear may all be
                                    factors to consider when deciding on the degree of slip-resistance needed for a given area.
                                </p>
                                <p>
                                    MARBLELIFE or the product Manufacturer will not be responsible for injuries incurred in a slip and fall incident.
                                    It is the responsibility of the Owner/Buyer to provide for their own safety and to determine the suitability of these
                                    surfaces for their particular application.
                                </p>
                                <p>
                                    It is agreed that MARBLELIFE proposal inclusive of these terms and conditions will be added as an addendum to the
                                    Buyer/Owner contract.
                                </p>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>', b'0');

INSERT INTO `termsAndCondition` ( `TyepeId`,`TermAndCondition`,`IsDeleted`) VALUES ('241','<style>
ul#hide{
list-style:none
}
</style>
<table style="width:100%;page-break-after:avoid" cellpadding="0" cellspacing="0">
                
                 <tbody>
                     <tr>
                         <td style="vertical-align:top;max-width:50%;width:50%;min-width:50%">
                             <div>
                                 <ul id="hide">
                                     <li>
                                         Your MARBLELIFE Representative has provided you with an exact description and estimate of the cost of the services to be provided.  This includes pointing out for you the existing damage to the surfaces you desire to restore.  Special conditions may exist and will be noted.
                                     </li>
                                 </ul>
 
                                 <ul id="hide">
                                     <li>
                                         <b> FURNITURE AND PLUMBING -</b> 
                                         Unless you are otherwise notified by the MARBLELIFE Representative, our Technicians are not trained,
                                         equipped, licensed or authorized to do plumbing or furniture moving.  If they remove or replace fixtures at your request, it is done solely
                                         as a courtesy, and the responsibility for any damage is yours.
                                     </li>
                                 </ul>
 
                                 <ul id="hide">
                                     <li>
                                         <b>PAINT AND WALLPAPER-</b>  
                                         If you are re-painting or re-wallpapering, please wait until after service has been completed. In order to protect paint and wallpaper,
                                         we must mask it.  When it is removed, masking tape occasionally pulls off small pieces of the surface to which it is stuck.
                                         If you wish to preserve the existing wallpaper or paint, please alert the craftsman, but even then, damage may occur.
                                         If so, we are not responsible for replacing your wallpaper or touching up paint.
                                     </li>
                                 </ul>
                                 <ul id="hide">
                                     <li>
                                         <b>GROUT -</b> 
                                         Some stains on grout will remain unless you have specifically contracted for its restoration, stripping, replacement or
                                         colorsealing.
                                     </li>
                                 </ul>
                                 <ul id="hide">
                                     <li>
                                         <b>STAINS -</b> Some stains on tile will remain unless you have specifically contracted for their removal or reduction.
                                         Stain removal cannot be guaranteed.  Poulticing services may require repeated application in order to remove or reduce stains.
                                         Each application is a separate service charge and visit.
 
                                     </li>
                                 </ul>
                                 <ul id="hide">
                                     <li>
                                         <b>PAYMENT TERMS  - </b>DUE ON COMPLETION
                                     </li>
                                 </ul>
                                 <ul id="hide">
                                     <li>
                                         <b>DEPOSIT-</b> A 50% deposit is required at time of scheduling
                                     </li>
                                 </ul>
                                 <ul id="hide">
                                     <li>
                                         We accept CASH, CHECKS, VISA or MASTERCARD.  On the final day of the job, the Craftsman will present you with an invoice for the work.
                                         Please plan to be there on the final day to inspect the work and take care of payment or arrange in advance for your check or payment to be
                                         processed  when the job is completed
                                     </li>
                                 </ul>
                                 <ul id="hide"><li id="hide"><b>RESPONSIBILITIES OF THE CUSTOMERS</b></li></ul>
                                 <ul style="margin-left: 15px !important;">
                                     <li>
                                         Customers will make work site available to MARBLELIFE on the days and times indicated in the work agreement.
                                         Customer will notify MARBLELIFE a minimum of 7 DAYS in advance if work site cannot be made available.  If no notification is provided that
                                         the job site is not available, customer will still be charged for the day of service plus associated travel charges to and from the site.
                                     </li>
                                 </ul>
                                 <ul style="margin-left: 15px !important;">
                                     <li>
                                         Customer will provide access and assistance to electrical resources, on-site parking, water, and drain at job site.
                                     </li>
                                 </ul>
                                 <ul style="margin-left: 15px !important;">
                                     <li>
                                         Customer will arrange necessary security at the work site to ensure workers are not interrupted during the performance of their duties.
                                     </li>
                                 </ul>
                                 <ul style="margin-left: 15px !important;">
                                     <li>
                                         In the event that work completed by MARBLELIFE is then damaged or disturbed by another contractor, MARBLELIFE will bill for the additional
                                         costs associated with repairing or correcting the situation.  This is particularly relevant to the application of sealers or topical treatments.
 
                                     </li>
                                 </ul>
                                 <ul style="margin-left: 15px !important;">
                                     <li>
                                         Unless otherwise stated, the customer executing this agreement will be deemed responsible for signing off on work completed. Once signed off the
                                         project is deemed to be completed.  Any additional work defined will be done under a separate agreement as a new project
                                     </li>
                                 </ul>
 
 
                                 <ul style="margin-left: 15px !important;">
                                     <li>
                                         Customer will remove all furniture, fixtures, rugs, and other obstacles from worksite before the work is performed and return them after each
                                         work session is complete.  This agreement does not include charges for moving equipment, furnishings or any other material that may interfere
                                         with access to the surface being worked on.  In the event, that the craftsman must move furniture or any other items, additional costs may be
                                         charged.  Minimum moving costs are $75.  Additional charges per piece will apply.  Please be advised, MARBLELIFE is not a moving company, and
                                         does not equip their craftsman with moving equipment.
                                     </li>
                                 </ul>
 
                                 <ul style="margin-left: 15px !important;">
                                     <li>
                                         Every effort will be made to avoid contact with or damage to woodwork, however MARBLE LIFE may need to address surfaces in
                                         immediate contact with a wood surface.  MARBLELIFE is not responsible for any damage to woodwork during cleaning or stripping process.
                                         Any repainting that needs to be done will be customer responsibility.  It is understood that the nature of stripping a surface is remove
                                         the coating from that surface.
                                     </li>
                                 </ul>
 
                                  
                             </div>
                         </td>
                         <td style="vertical-align:top;max-width:50%;width:50%;min-width:50%">
                             <div>
                                 <ul id="hide">
								 <li>
                                     <b>MATERIALS SPECIFICATIONS-</b>
                                         All material used in the performance of the work agreed upon in this work agreement will meet or
                                         exceed applicable industry standards.  SDS sheets will be provided upon request for all chemicals used in the performance of the work.
										 MARBLELIFE ensures that these materials will meet Federal and State government standards and will meet Federal and State laws.
                                         These materials include, but are not limited to:  Fungicide, Cleaning Chemicals, Polishes, Mastic, Sealers etc.
                                     </li>         
                                 </ul>
                                 <ul id="hide">
                                     <li>
                                         <b>PERSONNEL QUALIFICATIONS-</b> 
                                         All work outlined in this agreement will be done by MARBLELIFE personnel.  All personnel are trained by MARBLELIFE or its designee and
                                         are full capable of performing the work agreed upon.  MARBLELIFE personnel will perform the work in a professional manner, will wear MARBLELIFE
                                         standard work attire, and will attempt to the best of their ability not to disrupt the normal activities of the customer.
                                     </li>
                                 </ul>
 
 
                                 <ul id="hide">
                                     <li>
                                         <b>COMPLIANCE WITH THE LAWS-</b> 
                                         In execution of this agreement, MARBLELIFE will abide by all the existing laws, codes, rules, and regulations set forth by all relevant
                                         authorities having jurisdiction in the location of the work site.
                                     </li>
                                 </ul>
 
 
                                 <ul id="hide">
                                     <li>
                                         <b>CUSTOMER RIGHT TO INSPECT WORK-</b> 
                                         Customer reserves the right to inspect the work site determine that the quality of work is as agreed upon in this work agreement.
                                         This inspection shall take place at the conclusion of the performance of the work done by MARBLELIFE and after MARBLELIFE has notified the
                                         customer that said work has been completed.  Customer shall have the right to inspect said work no later than the scheduled completion date
                                         indicated in the contract.  In case of a long-term preservation agreement, the customer and MARBLELIFE shall agree in advance to regular quality
                                         audits.
                                     </li>
                                 </ul>
 
 
                                 <ul id="hide">
                                     <li>
                                         <b>LEGAL EXPENSES-</b> 
                                         In the event that payment for services are not made within the agreed upon payment terms, Customer will be responsible for late fees,
                                         interest charges, legal fees, and any collection fees required to secure payment
                                     </li>
                                 </ul>
 
                                 <ul id="hide">
                                     <li>
                                         In the event of any litigation arising out of this contract, the losing party will be responsible for the attorney fees and costs of the
                                         prevailing party, including any such fees or costs incurred on appeal.
 
                                     </li>
                                 </ul>
 
                                 <ul id="hide">
                                     <li>
                                         <b>WAIVER OF JURY TRIAL-</b> 
                                         EACH OF THE PARTIES HERETO WAIVES ITS RESPECTIVE RIGHTS TO A TRIAL BY JURY OF ANY CLAIM OR CAUSE OF ACTION BASED UPON OR ARISING OUT OF OR
                                         RELATED TO THIS CONTRACT OR THE TRANSACTION CONTEMPLATED HEREBY OR THEREBY IN ANY ACTION, PROCEEDING OR OTHER LITIGATION OF ANY TYPE BROUGHT BY
                                         ANY PARTY AGAINST THE OTHER PARTY, WHETHER WITH RESPECT TO CONTRACT CLAIMS, TORT CLAIMS, OR OTHERWISE. EACH OF THE PARTIES HERETO AGREES THAT ANY
                                         SUCH CLAIM OR CAUSE OF ACTION SHALL BE TRIED BY A COURT TRIAL WITHOUT A JURY.
 
                                     </li>
                                 </ul>
 
                                 <ul id="hide">
                                     <li>
                                         <b>INSURANCE-</b> 
                                         All MARBLELIFE employees are covered by $1,000,000.00 comprehensive general liability insurance, and have full Worker
                                         Compensation coverage.  It will take approximately 5 to 7 working days to process a Certificate of Insurance.
                                     </li>
                                 </ul>
 
                                 <ul id="hide">
                                     <li>
  <b>THE MARBLELIFE GUARANTEE-</b> 
                                         Subject to limitations set forth under the MARBLELIFE Standard Terms and Conditions, the MARBLELIFE office. identified herein guarantees the
                                         results of the services performed, and if, within two (2) days after the performance of the services performed, the customer notifies the
                                         MARBLELIFE office referenced herein of dissatisfaction with the results of the services performed, MARBLELIFE shall reapply the service during
                                         the next agreed upon service period or at another time mutually agreed upon, or shall refund the relevant portion of the service fee paid by
                                         customer.
                                     </li>
                                 </ul>
                             </div>
                         </td>
                     </tr>
                 </tbody>
             </table>', b'0');



             UPDATE `makalu`.`termsandcondition` SET `TermAndCondition` = '<style>
 ul#hide{
 list-style:none
 }
 </style>
 <table style="width:100%;page-break-after:avoid" cellpadding="0" cellspacing="0">
                 
                  <tbody>
                      <tr>
                          <td style="vertical-align:top;max-width:50%;width:50%;min-width:50%">
                              <div>
                                  <ul id="hide">
                                      <li>
                                          These Terms and Conditions govern all contracts for marble, stone, tile, and other organic and inorganic surface restoration and preservation services performed, and products offered by, MARBLELIFE, or its franchisees and affiliates. Your MARBLELIFE Representative has provided you with an exact description and estimate of the cost of the services to be provided. This includes pointing out for you the existing damage to the surfaces you desire to restore. Special conditions may exist and will be noted.
                                      </li>
                                  </ul>
  
                                  <ul id="hide">
                                      <li>
                                          <b> FURNITURE AND PLUMBING -</b> 
                                          Unless you are otherwise notified by the MARBLELIFE Representative,
										  our technicians are not trained, equipped, licensed or authorized to 
										  do plumbing or furniture moving. If they remove or replace fixtures 
										  at your request, it is done solely as a courtesy, and the responsibility 
										  for any damage is yours.
                                      </li>
                                  </ul>
  
                                  <ul id="hide">
                                      <li>
                                          <b>PAINT AND WALLPAPER-</b>  
                                          If you are re-painting or re-wallpapering, please wait until after 
										  service has been completed. In order to protect paint and wallpaper,
										  we must mask it. When it is removed, masking tape occasionally pulls
										  off small pieces of the surface to which it is stuck. If you wish to 
										  preserve the existing wallpaper or paint, please alert the craftsman,
										  but even then, damage may occur. If so, we are not responsible for 
										  replacing your wallpaper or touching up paint.
                                      </li>
                                  </ul>
                                  <ul id="hide">
                                      <li>
                                          <b>GROUT -</b> 
                                          Some stains on grout will remain unless you have specifically 
										  contracted for its restoration, stripping, replacement or 
										  colorsealing.
                                      </li>

                                  </ul>
                                  <ul id="hide">
                                      <li>
                                          <b>STAINS -</b> Some stains on tile will remain unless you have 
										  specifically contracted for their removal or reduction. Stain removal
										  cannot be guaranteed. Poulticing services may require repeated 
										  application in order to remove or reduce stains. Each application is 
										  a separate service charge and visit.
  
                                      </li>
                                  </ul>
								  <ul id="hide">
                                      <li>
                                          <b>SITE ACCESS AND CONDITIONS -</b> You shall grant to, or obtain for, 
										  MARBLELIFE unimpeded access to the site for all equipment and personnel 
										  necessary for the performance of the services to be provided, and access 
										  necessary for MARBLELIFE’S personnel to photograph the site and document 
										  the conditions. Such access to and photograph by MARBLELIFE personal of 
										  the site shall be granted both before completion of MARBLELIFE’s services
										  on the site and after completion of MARBLELIFE’s services on the site. As 
										  required to effectuate such access, you shall notify all owners, lessees, 
										  contractors, subcontractors, and other possessors of the site that 
										  MARBLELIFE must be allowed free access to the site.
  
                                      </li>
                                  </ul>
                                  <ul id="hide">
                                      <li>
                                          <b>PAYMENT TERMS  - </b>DUE ON COMPLETION<br/>
										  In consideration for the performance of the services provided, MARBLELIFE shall be paid an amount and according to terms set forth in the estimate of the cost of services (“Estimate”).  However, if payment terms are not listed in the Estimate, payment for services provided shall be payable upon receipt of MARBLELIFE’S invoice date (the “Payment Due Date”). All payments must be paid by the Payment Due Date, and shall not be contingent upon Customer’s receipt of separate payment, financing, receipt of insurance proceeds or other conditions whatsoever. If Customer objects to any portion of an invoice, it shall notify MARBLELIFE in writing within two (2) days from the date of actual receipt of the invoice of the amount and nature of the dispute, and shall timely pay undisputed portions of the invoice. Past due invoices and any sums improperly withheld by Customer shall accrue interest thereon at the rate of one percent (1.5%) per month, or the maximum rate allowed by law, whichever is lower. Customer agrees to pay all costs and expenses, including reasonable attorney’s fees and costs, incurred by MARBLELIFE should collection proceedings be necessary to collect on Customer’s overdue account. Unless the Estimate specifies the cost of services as not-to-exceed or lump sum, Customer acknowledges that any cost estimates and schedules provided by MARBLELIFE may be subject to change based upon the actual site conditions encountered, weather delays and impact and any other requirements of the Customer and should be used by Customer for planning purposes only.
MARBLELIFE will endeavor to perform the services within the Estimate, but will notify Customer if estimates are likely to be exceeded. In the event of changed site conditions or other conditions requiring additional time, Customer agrees to pay the reasonable and necessary increases resulting from such additional time.  Unless otherwise specified in the Estimate, Customer will be solely responsible for all applicable federal, state or local duty, import, sales, use, business, occupation, gross receipts or similar tax on the services provided.
                                      </li>
                                  </ul>
                                  <ul id="hide">
                                      <li>
                                          <b>DEPOSIT-</b> A 50% deposit is required at time of scheduling, unless limited by law, in which case the maximum deposit allowed by law will be required up to 50%.  
We accept CASH, CHECKS, VISA or MASTERCARD. On the final day of the job, the Craftsman will present you with an invoice for the work. Please plan to be there on the final day to inspect the work and take care of payment or arrange in advance for your check or payment to be processed when the job is completed
                                      </li>
                                  </ul>
                                  
                                  <ul id="hide"><li id="hide"><b>RESPONSIBILITIES OF THE CUSTOMERS</b></li></ul>
                                  <ul id="hide">
                                      <li>
                                          Customers will make work site available to MARBLELIFE on the days 
										  and times indicated in the work agreement. Customer must also make 
										  the work site available to MARBLELIFE for a minimum of eight (8) 
										  continuous hours on the days indicated in the work agreement.  
										  Customer will notify MARBLELIFE a minimum of 7 DAYS in advance if 
										  work site cannot be made available. If no notification is provided 
										  that the job site is not available, customer will still be charged 
										  for the day of service plus associated travel charges to and from 
										  the site.<br/>
										  Customer shall provide or otherwise make available to MARBLELIFE all information in 
										  its possession or subject to its control regarding existing and proposed conditions 
										  at the site. Customer shall immediately transmit to MARBLELIFE any new information 
										  concerning site conditions that becomes available, and any change in plans or 
										  specifications concerning the site or the services provided to the extent such 
										  information may affect MARBLELIFE’S performance of the services provided.Customer 
										  will provide access and assistance to electrical resources, on-site parking, water, 
										  and drain at job site. If the site requires on-site paid parking costs, such parking
										  costs shall be added as an additional expense to Customer’s invoice for MARBLELIFE
										  services.
                                      </li>
                                  </ul>
                                  
                                  
  
                                   
                              </div>
                          </td>
                          <td style="vertical-align:top;max-width:50%;width:50%;min-width:50%">
                              <div>
							  <ul id="hide">
							  <li>Customer will arrange necessary security at the work site to ensure workers 
							  are not interrupted during the performance of their duties.<br/>
							  In the event that work completed by MARBLELIFE is then damaged or disturbed by 
							  another contractor, MARBLELIFE will bill for the additional costs associated 
							  with repairing or correcting the situation. This is particularly relevant to 
							  the application of sealers or topical treatments and coatings Unless otherwise 
							  stated, the customer executing this agreement will be deemed responsible for 
							  signing off on work completed. Once signed off the project is deemed to be 
							  completed. Any additional work defined will be done under a separate agreement 
							  as a new project Customer will remove all furniture, fixtures, rugs, and other 
							  obstacles from worksite before the work is performed and return them after each 
							  work session is complete. This agreement does not include charges for moving 
							  equipment, furnishings or any other material that may interfere with access to 
							  the surface being worked on. In the event, that the craftsman must move furniture
							  or any other items, additional costs may be charged. Minimum moving costs are $75.
							  Additional per piece charges will apply. Please be advised, MARBLELIFE is not a 
							  moving company, and does not equip their craftsman with moving equipment.Every 
							  effort will be made to avoid contact with or damage to woodwork, however MARBLE
							  LIFE may need to address surfaces in immediate contact with a wood surface. 
							  MARBLELIFE is not responsible for any damage to woodwork during cleaning or 
							  stripping process. Any repainting that needs to be done will be customer’s 
							  responsibility. It is understood that the nature of stripping a surface is 
							  remove the coating from that surface.
							  </li>
							  </ul>
                                  <ul id="hide">
 								 <li>
                                      <b>MATERIALS SPECIFICATIONS-</b>
                                          All material used in the performance of the work agreed upon in 
										  this work agreement will meet or exceed applicable industry standards.
										  SDS sheets will be provided upon request for all chemicals used in 
										  the performance of the work. MARBLELIFE ensures that these materials
										  will meet Federal and State government standards and will meet 
										  Federal and State laws. These materials include, but are not limited
										  to: Fungicide, Cleaning Chemicals, Polishes, Mastic, Sealers etc.
                                      </li>         
                                  </ul>
                                  <ul id="hide">
                                      <li>
                                          <b>PERSONNEL QUALIFICATIONS-</b> 
                                          All work outlined in this agreement will be done by MARBLELIFE 
										  personnel. All personnel are trained by MARBLELIFE or its designee
										  and are fully capable of performing the work agreed upon. MARBLELIFE
										  personnel will perform the work in a professional manner, will wear 
										  MARBLELIFE standard work attire approved by the local office 
										  management, and will attempt to the best of their ability not to 
										  disrupt the normal activities of the customer.
                                      </li>
                                  </ul>
  
  
                                  <ul id="hide">
                                      <li>
                                          <b>COMPLIANCE WITH THE LAWS-</b> 
                                          In execution of this agreement, MARBLELIFE will abide by all the existing laws, codes, rules, and regulations set forth by all relevant
                                          authorities having jurisdiction in the location of the work site.
                                      </li>
                                  </ul>
  
  
                                  <ul id="hide">
                                      <li>
                                          <b>CUSTOMER RIGHT TO INSPECT WORK-</b> 
                                          Customer reserves the right to inspect the work site determine that the quality of work is as agreed upon in this work agreement.
                                          This inspection shall take place at the conclusion of the performance of the work done by MARBLELIFE and after MARBLELIFE has notified the
                                          customer that said work has been completed.  Customer shall have the right to inspect said work no later than the scheduled completion date
                                          indicated in the contract.  In case of a long-term preservation agreement, the customer and MARBLELIFE shall agree in advance to regular quality
                                          audits.
                                      </li>
                                  </ul>
  
  
                                  <ul id="hide">
                                      <li>
                                          <b>LEGAL EXPENSES-</b> 
                                           In the event that payment for services are not made within the 
										  agreed upon payment terms, Customer will be responsible for late 
										  fees, interest charges, legal fees, and any collection fees required
										  to secure payment.<br/>
										  In the event of any litigation arising out of this contract, the 
										  losing party will be responsible for the attorney fees and costs of
										  the prevailing party, including any such fees or costs incurred on 
										  appeal.
                                      </li>
                                  </ul>
  
  <ul id="hide">
                                      <li>
                                          <b>RISK ALLOCATION AND LIMITATION OF LIABILITY-</b> 
                                          The parties acknowledge that a variety of risks potentially affect
										  MARBLELIFE by virtue of entering into an agreement to perform the 
										  services provided. The parties further acknowledge and agree that 
										  there is no disparity in bargaining power between the parties.
IN ORDER FOR CUSTOMER TO OBTAIN THE BENEFIT OF A LOWER FEE THAN WOULD OTHERWISE BE AVAILABLE, CUSTOMER 
AGREES TO LIMIT MARBLELIFE’S LIABILITY TO CUSTOMER, AND TO ANY AND ALL OTHER THIRD PARTIES, FOR CLAIMS 
ARISING OUT OF OR IN ANY WAY RELATED TO THE SERVICES PERFORMED OR TO BE PERFORMED BY MARBLELIFE. ACCORDINGLY, 
THE CUSTOMER AGREES THAT THE TOTAL AGGREGATE LIABILITY OF MARBLELIFE SHALL NOT EXCEED THE TOTAL FEE FOR THE 
SERVICES RENDERED ON THE PROJECT, OR $2,500, WHICHEVER IS LOWER, FOR ANY LIABILITIES, INCLUDING BUT NOT 
LIMITED TO NEGLIGENCE, ERRORS OR OMISSIONS, OR CONTRACT CLAIMS, AND CUSTOMER AGREES TO INDEMNIFY MARBLELIFE 
FOR ALL LIABILITIES IN EXCESS OF THE MONETARY LIMITS ESTABLISHED HEREIN.<br/>
Customer agrees that in no instance shall MARBLELIFE be responsible, in total or in part, for the errors or 
omissions of any other professional, contractor, subcontractor or any other third party. Customer also agrees
 that MARBLELIFE shall not be responsible for the means, methods, procedures, performance, quality or safety 
 of the construction contractors or subcontractors, or for their errors or omissions.
  
                                      </li>
                                  </ul>
                                  
  
                                  <ul id="hide">
                                      <li>
                                          <b>WAIVER OF JURY TRIAL-</b> 
                                          EACH OF THE PARTIES HERETO WAIVES ITS RESPECTIVE RIGHTS TO A TRIAL BY JURY OF ANY CLAIM OR CAUSE OF ACTION BASED UPON OR ARISING OUT OF OR
                                          RELATED TO THIS CONTRACT OR THE TRANSACTION CONTEMPLATED HEREBY OR THEREBY IN ANY ACTION, PROCEEDING OR OTHER LITIGATION OF ANY TYPE BROUGHT BY
                                          ANY PARTY AGAINST THE OTHER PARTY, WHETHER WITH RESPECT TO CONTRACT CLAIMS, TORT CLAIMS, OR OTHERWISE. EACH OF THE PARTIES HERETO AGREES THAT ANY
                                          SUCH CLAIM OR CAUSE OF ACTION SHALL BE TRIED BY A COURT TRIAL WITHOUT A JURY.
  
                                      </li>
                                  </ul>
  
                                  <ul id="hide">
                                      <li>
                                          <b>INSURANCE-</b> 
                                          All MARBLELIFE employees are covered by $1,000,000.00 comprehensive general liability insurance, and have full Worker
                                          Compensation coverage.  It will take approximately 5 to 7 working days to process a Certificate of Insurance.
                                      </li>
                                  </ul>
  
                                  <ul id="hide">
                                      <li>
   <b>THE MARBLELIFE GUARANTEE-</b> 
                                          Subject to limitations set forth under the MARBLELIFE Standard Terms and Conditions, the MARBLELIFE office. identified herein guarantees the
                                          results of the services performed, and if, within two (2) days after the performance of the services performed, the customer notifies the
                                          MARBLELIFE office referenced herein of dissatisfaction with the results of the services performed, MARBLELIFE shall reapply the service during
                                          the next agreed upon service period or at another time mutually agreed upon, or shall refund the relevant portion of the service fee paid by
                                          customer.
                                      </li>
                                  </ul>
                              </div>
                          </td>
                      </tr>
                  </tbody>
              </table>' WHERE (`TyepeId` = '241');




UPDATE `makalu`.`termsandcondition` SET `TermAndCondition` = '<style>\n  ul#hide{\n  list-style:none\n  }\n  </style>\n  <table style=\"width:100%;page-break-after:avoid\" cellpadding=\"0\" cellspacing=\"0\">\n                  \n                   <tbody>\n                       <tr>\n                           <td style=\"vertical-align:top;max-width:50%;width:50%;min-width:50%\">\n                               <div>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           These Terms and Conditions govern all contracts for marble, stone, tile, and other organic and inorganic surface restoration and preservation services performed, and products offered by, MARBLELIFE, or its franchisees and affiliates. Your MARBLELIFE Representative has provided you with an exact description and estimate of the cost of the services to be provided. This includes pointing out for you the existing damage to the surfaces you desire to restore. Special conditions may exist and will be noted.\n                                       </li>\n                                   </ul>\n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b> FURNITURE AND PLUMBING -</b> \n                                           Unless you are otherwise notified by the MARBLELIFE Representative,\n 										  our technicians are not trained, equipped, licensed or authorized to \n 										  do plumbing or furniture moving. If they remove or replace fixtures \n 										  at your request, it is done solely as a courtesy, and the responsibility \n 										  for any damage is yours.\n                                       </li>\n                                   </ul>\n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>PAINT AND WALLPAPER-</b>  \n                                           If you are re-painting or re-wallpapering, please wait until after \n 										  service has been completed. In order to protect paint and wallpaper,\n 										  we must mask it. When it is removed, masking tape occasionally pulls\n 										  off small pieces of the surface to which it is stuck. If you wish to \n 										  preserve the existing wallpaper or paint, please alert the craftsman,\n 										  but even then, damage may occur. If so, we are not responsible for \n 										  replacing your wallpaper or touching up paint.\n                                       </li>\n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>GROUT -</b> \n                                           Some stains on grout will remain unless you have specifically \n 										  contracted for its restoration, stripping, replacement or \n 										  colorsealing.\n                                       </li>\n \n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>STAINS -</b> Some stains on tile will remain unless you have \n 										  specifically contracted for their removal or reduction. Stain removal\n 										  cannot be guaranteed. Poulticing services may require repeated \n 										  application in order to remove or reduce stains. Each application is \n 										  a separate service charge and visit.\n   \n                                       </li>\n                                   </ul>\n 								  <ul id=\"hide\">\n                                       <li>\n                                           <b>SITE ACCESS AND CONDITIONS -</b> You shall grant to, or obtain for, \n 										  MARBLELIFE unimpeded access to the site for all equipment and personnel \n 										  necessary for the performance of the services to be provided, and access \n 										  necessary for MARBLELIFE personnel to photograph the site and document \n 										  the conditions. Such access to and photograph by MARBLELIFE personal of \n 										  the site shall be granted both before completion of MARBLELIFE services\n 										  on the site and after completion of MARBLELIFE services on the site. As \n 										  required to effectuate such access, you shall notify all owners, lessees, \n 										  contractors, subcontractors, and other possessors of the site that \n 										  MARBLELIFE must be allowed free access to the site.\n   \n                                       </li>\n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>PAYMENT TERMS  - </b>DUE ON COMPLETION<br/>\n 										  In consideration for the performance of the services provided, MARBLELIFE shall be paid an amount and according to terms set forth in the estimate of the cost of services (Estimate).  However, if payment terms are not listed in the Estimate, payment for services provided shall be payable upon receipt of MARBLELIFE invoice date (the Payment Due Date). All payments must be paid by the Payment Due Date, and shall not be contingent upon Customer’s receipt of separate payment, financing, receipt of insurance proceeds or other conditions whatsoever. If Customer objects to any portion of an invoice, it shall notify MARBLELIFE in writing within two (2) days from the date of actual receipt of the invoice of the amount and nature of the dispute, and shall timely pay undisputed portions of the invoice. Past due invoices and any sums improperly withheld by Customer shall accrue interest thereon at the rate of one percent (1.5%) per month, or the maximum rate allowed by law, whichever is lower. Customer agrees to pay all costs and expenses, including reasonable attorney’s fees and costs, incurred by MARBLELIFE should collection proceedings be necessary to collect on Customer’s overdue account. Unless the Estimate specifies the cost of services as not-to-exceed or lump sum, Customer acknowledges that any cost estimates and schedules provided by MARBLELIFE may be subject to change based upon the actual site conditions encountered, weather delays and impact and any other requirements of the Customer and should be used by Customer for planning purposes only.\n MARBLELIFE will endeavor to perform the services within the Estimate, but will notify Customer if estimates are likely to be exceeded. In the event of changed site conditions or other conditions requiring additional time, Customer agrees to pay the reasonable and necessary increases resulting from such additional time.  Unless otherwise specified in the Estimate, Customer will be solely responsible for all applicable federal, state or local duty, import, sales, use, business, occupation, gross receipts or similar tax on the services provided.\n                                       </li>\n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>DEPOSIT-</b> A 50% deposit is required at time of scheduling, unless limited by law, in which case the maximum deposit allowed by law will be required up to 50%.  \n We accept CASH, CHECKS, VISA or MASTERCARD. On the final day of the job, the Craftsman will present you with an invoice for the work. Please plan to be there on the final day to inspect the work and take care of payment or arrange in advance for your check or payment to be processed when the job is completed\n                                       </li>\n                                   </ul>\n                                   \n                                   <ul id=\"hide\"><li id=\"hide\"><b>RESPONSIBILITIES OF THE CUSTOMERS</b></li></ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           Customers will make work site available to MARBLELIFE on the days \n 										  and times indicated in the work agreement. Customer must also make \n 										  the work site available to MARBLELIFE for a minimum of eight (8) \n 										  continuous hours on the days indicated in the work agreement.  \n 										  Customer will notify MARBLELIFE a minimum of 7 DAYS in advance if \n 										  work site cannot be made available. If no notification is provided \n 										  that the job site is not available, customer will still be charged \n 										  for the day of service plus associated travel charges to and from \n 										  the site.<br/>\n 										  Customer shall provide or otherwise make available to MARBLELIFE all information in \n 										  its possession or subject to its control regarding existing and proposed conditions \n 										  at the site. Customer shall immediately transmit to MARBLELIFE any new information \n 										  concerning site conditions that becomes available, and any change in plans or \n 										  specifications concerning the site or the services provided to the extent such \n 										  information may affect MARBLELIFE performance of the services provided.Customer \n 										  will provide access and assistance to electrical resources, on-site parking, water, \n 										  and drain at job site. If the site requires on-site paid parking costs, such parking\n 										  costs shall be added as an additional expense to Customer’s invoice for MARBLELIFE\n 										  services.\n                                       </li>\n                                   </ul>\n                                   \n                                   \n   \n                                    \n                               </div>\n                           </td>\n                           <td style=\"vertical-align:top;max-width:50%;width:50%;min-width:50%\">\n                               <div>\n 							  <ul id=\"hide\">\n 							  <li>Customer will arrange necessary security at the work site to ensure workers \n 							  are not interrupted during the performance of their duties.<br/>\n 							  In the event that work completed by MARBLELIFE is then damaged or disturbed by \n 							  another contractor, MARBLELIFE will bill for the additional costs associated \n 							  with repairing or correcting the situation. This is particularly relevant to \n 							  the application of sealers or topical treatments and coatings Unless otherwise \n 							  stated, the customer executing this agreement will be deemed responsible for \n 							  signing off on work completed. Once signed off the project is deemed to be \n 							  completed. Any additional work defined will be done under a separate agreement \n 							  as a new project Customer will remove all furniture, fixtures, rugs, and other \n 							  obstacles from worksite before the work is performed and return them after each \n 							  work session is complete. This agreement does not include charges for moving \n 							  equipment, furnishings or any other material that may interfere with access to \n 							  the surface being worked on. In the event, that the craftsman must move furniture\n 							  or any other items, additional costs may be charged. Minimum moving costs are $75.\n 							  Additional per piece charges will apply. Please be advised, MARBLELIFE is not a \n 							  moving company, and does not equip their craftsman with moving equipment.Every \n 							  effort will be made to avoid contact with or damage to woodwork, however MARBLE\n 							  LIFE may need to address surfaces in immediate contact with a wood surface. \n 							  MARBLELIFE is not responsible for any damage to woodwork during cleaning or \n 							  stripping process. Any repainting that needs to be done will be customer’s \n 							  responsibility. It is understood that the nature of stripping a surface is \n 							  remove the coating from that surface.\n 							  </li>\n 							  </ul>\n                                   <ul id=\"hide\">\n  								 <li>\n                                       <b>MATERIALS SPECIFICATIONS-</b>\n                                           All material used in the performance of the work agreed upon in \n 										  this work agreement will meet or exceed applicable industry standards.\n 										  SDS sheets will be provided upon request for all chemicals used in \n 										  the performance of the work. MARBLELIFE ensures that these materials\n 										  will meet Federal and State government standards and will meet \n 										  Federal and State laws. These materials include, but are not limited\n 										  to: Fungicide, Cleaning Chemicals, Polishes, Mastic, Sealers etc.\n                                       </li>         \n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>PERSONNEL QUALIFICATIONS-</b> \n                                           All work outlined in this agreement will be done by MARBLELIFE \n 										  personnel. All personnel are trained by MARBLELIFE or its designee\n 										  and are fully capable of performing the work agreed upon. MARBLELIFE\n 										  personnel will perform the work in a professional manner, will wear \n 										  MARBLELIFE standard work attire approved by the local office \n 										  management, and will attempt to the best of their ability not to \n 										  disrupt the normal activities of the customer.\n                                       </li>\n                                   </ul>\n   \n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>COMPLIANCE WITH THE LAWS-</b> \n                                           In execution of this agreement, MARBLELIFE will abide by all the existing laws, codes, rules, and regulations set forth by all relevant\n                                           authorities having jurisdiction in the location of the work site.\n                                       </li>\n                                   </ul>\n   \n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>CUSTOMER RIGHT TO INSPECT WORK-</b> \n                                           Customer reserves the right to inspect the work site determine that the quality of work is as agreed upon in this work agreement.\n                                           This inspection shall take place at the conclusion of the performance of the work done by MARBLELIFE and after MARBLELIFE has notified the\n                                           customer that said work has been completed.  Customer shall have the right to inspect said work no later than the scheduled completion date\n                                           indicated in the contract.  In case of a long-term preservation agreement, the customer and MARBLELIFE shall agree in advance to regular quality\n                                           audits.\n                                       </li>\n                                   </ul>\n   \n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>LEGAL EXPENSES-</b> \n                                            In the event that payment for services are not made within the \n 										  agreed upon payment terms, Customer will be responsible for late \n 										  fees, interest charges, legal fees, and any collection fees required\n 										  to secure payment.<br/>\n 										  In the event of any litigation arising out of this contract, the \n 										  losing party will be responsible for the attorney fees and costs of\n 										  the prevailing party, including any such fees or costs incurred on \n 										  appeal.\n                                       </li>\n                                   </ul>\n   \n   <ul id=\"hide\">\n                                       <li>\n                                           <b>RISK ALLOCATION AND LIMITATION OF LIABILITY-</b> \n                                           The parties acknowledge that a variety of risks potentially affect\n 										  MARBLELIFE by virtue of entering into an agreement to perform the \n 										  services provided. The parties further acknowledge and agree that \n 										  there is no disparity in bargaining power between the parties.\n IN ORDER FOR CUSTOMER TO OBTAIN THE BENEFIT OF A LOWER FEE THAN WOULD OTHERWISE BE AVAILABLE, CUSTOMER \n AGREES TO LIMIT MARBLELIFE LIABILITY TO CUSTOMER, AND TO ANY AND ALL OTHER THIRD PARTIES, FOR CLAIMS \n ARISING OUT OF OR IN ANY WAY RELATED TO THE SERVICES PERFORMED OR TO BE PERFORMED BY MARBLELIFE. ACCORDINGLY, \n THE CUSTOMER AGREES THAT THE TOTAL AGGREGATE LIABILITY OF MARBLELIFE SHALL NOT EXCEED THE TOTAL FEE FOR THE \n SERVICES RENDERED ON THE PROJECT, OR $2,500, WHICHEVER IS LOWER, FOR ANY LIABILITIES, INCLUDING BUT NOT \n LIMITED TO NEGLIGENCE, ERRORS OR OMISSIONS, OR CONTRACT CLAIMS, AND CUSTOMER AGREES TO INDEMNIFY MARBLELIFE \n FOR ALL LIABILITIES IN EXCESS OF THE MONETARY LIMITS ESTABLISHED HEREIN.<br/>\n Customer agrees that in no instance shall MARBLELIFE be responsible, in total or in part, for the errors or \n omissions of any other professional, contractor, subcontractor or any other third party. Customer also agrees\n  that MARBLELIFE shall not be responsible for the means, methods, procedures, performance, quality or safety \n  of the construction contractors or subcontractors, or for their errors or omissions.\n   \n                                       </li>\n                                   </ul>\n                                   \n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>WAIVER OF JURY TRIAL-</b> \n                                           EACH OF THE PARTIES HERETO WAIVES ITS RESPECTIVE RIGHTS TO A TRIAL BY JURY OF ANY CLAIM OR CAUSE OF ACTION BASED UPON OR ARISING OUT OF OR\n                                           RELATED TO THIS CONTRACT OR THE TRANSACTION CONTEMPLATED HEREBY OR THEREBY IN ANY ACTION, PROCEEDING OR OTHER LITIGATION OF ANY TYPE BROUGHT BY\n                                           ANY PARTY AGAINST THE OTHER PARTY, WHETHER WITH RESPECT TO CONTRACT CLAIMS, TORT CLAIMS, OR OTHERWISE. EACH OF THE PARTIES HERETO AGREES THAT ANY\n                                           SUCH CLAIM OR CAUSE OF ACTION SHALL BE TRIED BY A COURT TRIAL WITHOUT A JURY.\n   \n                                       </li>\n                                   </ul>\n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>INSURANCE-</b> \n                                           All MARBLELIFE employees are covered by $1,000,000.00 comprehensive general liability insurance, and have full Worker\n                                           Compensation coverage.  It will take approximately 5 to 7 working days to process a Certificate of Insurance.\n                                       </li>\n                                   </ul>\n   \n                                   <ul id=\"hide\">\n                                       <li>\n    <b>THE MARBLELIFE GUARANTEE-</b> \n                                           Subject to limitations set forth under the MARBLELIFE Standard Terms and Conditions, the MARBLELIFE office. identified herein guarantees the\n                                           results of the services performed, and if, within two (2) days after the performance of the services performed, the customer notifies the\n                                           MARBLELIFE office referenced herein of dissatisfaction with the results of the services performed, MARBLELIFE shall reapply the service during\n                                           the next agreed upon service period or at another time mutually agreed upon, or shall refund the relevant portion of the service fee paid by\n                                           customer.\n                                       </li>\n                                   </ul>\n                               </div>\n                           </td>\n                       </tr>\n                   </tbody>\n               </table>' WHERE (`TyepeId` = '241');


UPDATE `makalu`.`termsandcondition` SET `TermAndCondition` = '<style>\n  ul#hide{\n  list-style:none\n  }\n  </style>\n  <table style=\"width:100%;page-break-after:avoid\" cellpadding=\"0\" cellspacing=\"0\">\n                  \n                   <tbody>\n                       <tr>\n                           <td style=\"vertical-align:top;max-width:50%;width:50%;min-width:50%\">\n                               <div>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           These Terms and Conditions govern all contracts for marble, stone, tile, and other organic and inorganic surface restoration and preservation services performed, and products offered by, MARBLELIFE, or its franchisees and affiliates. Your MARBLELIFE Representative has provided you with an exact description and estimate of the cost of the services to be provided. This includes pointing out for you the existing damage to the surfaces you desire to restore. Special conditions may exist and will be noted.\n                                       </li>\n                                   </ul>\n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b> FURNITURE AND PLUMBING -</b> \n                                           Unless you are otherwise notified by the MARBLELIFE Representative,\n 										  our technicians are not trained, equipped, licensed or authorized to \n 										  do plumbing or furniture moving. If they remove or replace fixtures \n 										  at your request, it is done solely as a courtesy, and the responsibility \n 										  for any damage is yours.\n                                       </li>\n                                   </ul>\n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>PAINT AND WALLPAPER-</b>  \n                                           If you are re-painting or re-wallpapering, please wait until after \n 										  service has been completed. In order to protect paint and wallpaper,\n 										  we must mask it. When it is removed, masking tape occasionally pulls\n 										  off small pieces of the surface to which it is stuck. If you wish to \n 										  preserve the existing wallpaper or paint, please alert the craftsman,\n 										  but even then, damage may occur. If so, we are not responsible for \n 										  replacing your wallpaper or touching up paint.\n                                       </li>\n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>GROUT -</b> \n                                           Some stains on grout will remain unless you have specifically \n 										  contracted for its restoration, stripping, replacement or \n 										  colorsealing.\n                                       </li>\n \n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>STAINS -</b> Some stains on tile will remain unless you have \n 										  specifically contracted for their removal or reduction. Stain removal\n 										  cannot be guaranteed. Poulticing services may require repeated \n 										  application in order to remove or reduce stains. Each application is \n 										  a separate service charge and visit.\n   \n                                       </li>\n                                   </ul>\n 								  <ul id=\"hide\">\n                                       <li>\n                                           <b>SITE ACCESS AND CONDITIONS -</b> You shall grant to, or obtain for, \n 										  MARBLELIFE unimpeded access to the site for all equipment and personnel \n 										  necessary for the performance of the services to be provided, and access \n 										  necessary for MARBLELIFE personnel to photograph the site and document \n 										  the conditions. Such access to and photograph by MARBLELIFE personal of \n 										  the site shall be granted both before completion of MARBLELIFE services\n 										  on the site and after completion of MARBLELIFE services on the site. As \n 										  required to effectuate such access, you shall notify all owners, lessees, \n 										  contractors, subcontractors, and other possessors of the site that \n 										  MARBLELIFE must be allowed free access to the site.\n   \n                                       </li>\n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>PAYMENT TERMS  - </b>DUE ON COMPLETION<br/>\n 										  In consideration for the performance of the services provided, MARBLELIFE shall be paid an amount and according to terms set forth in the estimate of the cost of services (Estimate).  However, if payment terms are not listed in the Estimate, payment for services provided shall be payable upon receipt of MARBLELIFE invoice date (the Payment Due Date). All payments must be paid by the Payment Due Date, and shall not be contingent upon Customer receipt of separate payment, financing, receipt of insurance proceeds or other conditions whatsoever. If Customer objects to any portion of an invoice, it shall notify MARBLELIFE in writing within two (2) days from the date of actual receipt of the invoice of the amount and nature of the dispute, and shall timely pay undisputed portions of the invoice. Past due invoices and any sums improperly withheld by Customer shall accrue interest thereon at the rate of one percent (1.5%) per month, or the maximum rate allowed by law, whichever is lower. Customer agrees to pay all costs and expenses, including reasonable attorney fees and costs, incurred by MARBLELIFE should collection proceedings be necessary to collect on Customer overdue account. Unless the Estimate specifies the cost of services as not-to-exceed or lump sum, Customer acknowledges that any cost estimates and schedules provided by MARBLELIFE may be subject to change based upon the actual site conditions encountered, weather delays and impact and any other requirements of the Customer and should be used by Customer for planning purposes only.\n MARBLELIFE will endeavor to perform the services within the Estimate, but will notify Customer if estimates are likely to be exceeded. In the event of changed site conditions or other conditions requiring additional time, Customer agrees to pay the reasonable and necessary increases resulting from such additional time.  Unless otherwise specified in the Estimate, Customer will be solely responsible for all applicable federal, state or local duty, import, sales, use, business, occupation, gross receipts or similar tax on the services provided.\n                                       </li>\n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>DEPOSIT-</b> A 50% deposit is required at time of scheduling, unless limited by law, in which case the maximum deposit allowed by law will be required up to 50%.  \n We accept CASH, CHECKS, VISA or MASTERCARD. On the final day of the job, the Craftsman will present you with an invoice for the work. Please plan to be there on the final day to inspect the work and take care of payment or arrange in advance for your check or payment to be processed when the job is completed\n                                       </li>\n                                   </ul>\n                                   \n                                   <ul id=\"hide\"><li id=\"hide\"><b>RESPONSIBILITIES OF THE CUSTOMERS</b></li></ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           Customers will make work site available to MARBLELIFE on the days \n 										  and times indicated in the work agreement. Customer must also make \n 										  the work site available to MARBLELIFE for a minimum of eight (8) \n 										  continuous hours on the days indicated in the work agreement.  \n 										  Customer will notify MARBLELIFE a minimum of 7 DAYS in advance if \n 										  work site cannot be made available. If no notification is provided \n 										  that the job site is not available, customer will still be charged \n 										  for the day of service plus associated travel charges to and from \n 										  the site.<br/>\n 										  Customer shall provide or otherwise make available to MARBLELIFE all information in \n 										  its possession or subject to its control regarding existing and proposed conditions \n 										  at the site. Customer shall immediately transmit to MARBLELIFE any new information \n 										  concerning site conditions that becomes available, and any change in plans or \n 										  specifications concerning the site or the services provided to the extent such \n 										  information may affect MARBLELIFE performance of the services provided.Customer \n 										  will provide access and assistance to electrical resources, on-site parking, water, \n 										  and drain at job site. If the site requires on-site paid parking costs, such parking\n 										  costs shall be added as an additional expense to Customer invoice for MARBLELIFE\n 										  services.\n                                       </li>\n                                   </ul>\n                                   \n                                   \n   \n                                    \n                               </div>\n                           </td>\n                           <td style=\"vertical-align:top;max-width:50%;width:50%;min-width:50%\">\n                               <div>\n 							  <ul id=\"hide\">\n 							  <li>Customer will arrange necessary security at the work site to ensure workers \n 							  are not interrupted during the performance of their duties.<br/>\n 							  In the event that work completed by MARBLELIFE is then damaged or disturbed by \n 							  another contractor, MARBLELIFE will bill for the additional costs associated \n 							  with repairing or correcting the situation. This is particularly relevant to \n 							  the application of sealers or topical treatments and coatings Unless otherwise \n 							  stated, the customer executing this agreement will be deemed responsible for \n 							  signing off on work completed. Once signed off the project is deemed to be \n 							  completed. Any additional work defined will be done under a separate agreement \n 							  as a new project Customer will remove all furniture, fixtures, rugs, and other \n 							  obstacles from worksite before the work is performed and return them after each \n 							  work session is complete. This agreement does not include charges for moving \n 							  equipment, furnishings or any other material that may interfere with access to \n 							  the surface being worked on. In the event, that the craftsman must move furniture\n 							  or any other items, additional costs may be charged. Minimum moving costs are $75.\n 							  Additional per piece charges will apply. Please be advised, MARBLELIFE is not a \n 							  moving company, and does not equip their craftsman with moving equipment.Every \n 							  effort will be made to avoid contact with or damage to woodwork, however MARBLE\n 							  LIFE may need to address surfaces in immediate contact with a wood surface. \n 							  MARBLELIFE is not responsible for any damage to woodwork during cleaning or \n 							  stripping process. Any repainting that needs to be done will be customer \n 							  responsibility. It is understood that the nature of stripping a surface is \n 							  remove the coating from that surface.\n 							  </li>\n 							  </ul>\n                                   <ul id=\"hide\">\n  								 <li>\n                                       <b>MATERIALS SPECIFICATIONS-</b>\n                                           All material used in the performance of the work agreed upon in \n 										  this work agreement will meet or exceed applicable industry standards.\n 										  SDS sheets will be provided upon request for all chemicals used in \n 										  the performance of the work. MARBLELIFE ensures that these materials\n 										  will meet Federal and State government standards and will meet \n 										  Federal and State laws. These materials include, but are not limited\n 										  to: Fungicide, Cleaning Chemicals, Polishes, Mastic, Sealers etc.\n                                       </li>         \n                                   </ul>\n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>PERSONNEL QUALIFICATIONS-</b> \n                                           All work outlined in this agreement will be done by MARBLELIFE \n 										  personnel. All personnel are trained by MARBLELIFE or its designee\n 										  and are fully capable of performing the work agreed upon. MARBLELIFE\n 										  personnel will perform the work in a professional manner, will wear \n 										  MARBLELIFE standard work attire approved by the local office \n 										  management, and will attempt to the best of their ability not to \n 										  disrupt the normal activities of the customer.\n                                       </li>\n                                   </ul>\n   \n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>COMPLIANCE WITH THE LAWS-</b> \n                                           In execution of this agreement, MARBLELIFE will abide by all the existing laws, codes, rules, and regulations set forth by all relevant\n                                           authorities having jurisdiction in the location of the work site.\n                                       </li>\n                                   </ul>\n   \n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>CUSTOMER RIGHT TO INSPECT WORK-</b> \n                                           Customer reserves the right to inspect the work site determine that the quality of work is as agreed upon in this work agreement.\n                                           This inspection shall take place at the conclusion of the performance of the work done by MARBLELIFE and after MARBLELIFE has notified the\n                                           customer that said work has been completed.  Customer shall have the right to inspect said work no later than the scheduled completion date\n                                           indicated in the contract.  In case of a long-term preservation agreement, the customer and MARBLELIFE shall agree in advance to regular quality\n                                           audits.\n                                       </li>\n                                   </ul>\n   \n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>LEGAL EXPENSES-</b> \n                                            In the event that payment for services are not made within the \n 										  agreed upon payment terms, Customer will be responsible for late \n 										  fees, interest charges, legal fees, and any collection fees required\n 										  to secure payment.<br/>\n 										  In the event of any litigation arising out of this contract, the \n 										  losing party will be responsible for the attorney fees and costs of\n 										  the prevailing party, including any such fees or costs incurred on \n 										  appeal.\n                                       </li>\n                                   </ul>\n   \n   <ul id=\"hide\">\n                                       <li>\n                                           <b>RISK ALLOCATION AND LIMITATION OF LIABILITY-</b> \n                                           The parties acknowledge that a variety of risks potentially affect\n 										  MARBLELIFE by virtue of entering into an agreement to perform the \n 										  services provided. The parties further acknowledge and agree that \n 										  there is no disparity in bargaining power between the parties.\n IN ORDER FOR CUSTOMER TO OBTAIN THE BENEFIT OF A LOWER FEE THAN WOULD OTHERWISE BE AVAILABLE, CUSTOMER \n AGREES TO LIMIT MARBLELIFE LIABILITY TO CUSTOMER, AND TO ANY AND ALL OTHER THIRD PARTIES, FOR CLAIMS \n ARISING OUT OF OR IN ANY WAY RELATED TO THE SERVICES PERFORMED OR TO BE PERFORMED BY MARBLELIFE. ACCORDINGLY, \n THE CUSTOMER AGREES THAT THE TOTAL AGGREGATE LIABILITY OF MARBLELIFE SHALL NOT EXCEED THE TOTAL FEE FOR THE \n SERVICES RENDERED ON THE PROJECT, OR $2,500, WHICHEVER IS LOWER, FOR ANY LIABILITIES, INCLUDING BUT NOT \n LIMITED TO NEGLIGENCE, ERRORS OR OMISSIONS, OR CONTRACT CLAIMS, AND CUSTOMER AGREES TO INDEMNIFY MARBLELIFE \n FOR ALL LIABILITIES IN EXCESS OF THE MONETARY LIMITS ESTABLISHED HEREIN.<br/>\n Customer agrees that in no instance shall MARBLELIFE be responsible, in total or in part, for the errors or \n omissions of any other professional, contractor, subcontractor or any other third party. Customer also agrees\n  that MARBLELIFE shall not be responsible for the means, methods, procedures, performance, quality or safety \n  of the construction contractors or subcontractors, or for their errors or omissions.\n   \n                                       </li>\n                                   </ul>\n                                   \n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>WAIVER OF JURY TRIAL-</b> \n                                           EACH OF THE PARTIES HERETO WAIVES ITS RESPECTIVE RIGHTS TO A TRIAL BY JURY OF ANY CLAIM OR CAUSE OF ACTION BASED UPON OR ARISING OUT OF OR\n                                           RELATED TO THIS CONTRACT OR THE TRANSACTION CONTEMPLATED HEREBY OR THEREBY IN ANY ACTION, PROCEEDING OR OTHER LITIGATION OF ANY TYPE BROUGHT BY\n                                           ANY PARTY AGAINST THE OTHER PARTY, WHETHER WITH RESPECT TO CONTRACT CLAIMS, TORT CLAIMS, OR OTHERWISE. EACH OF THE PARTIES HERETO AGREES THAT ANY\n                                           SUCH CLAIM OR CAUSE OF ACTION SHALL BE TRIED BY A COURT TRIAL WITHOUT A JURY.\n   \n                                       </li>\n                                   </ul>\n   \n                                   <ul id=\"hide\">\n                                       <li>\n                                           <b>INSURANCE-</b> \n                                           All MARBLELIFE employees are covered by $1,000,000.00 comprehensive general liability insurance, and have full Worker\n                                           Compensation coverage.  It will take approximately 5 to 7 working days to process a Certificate of Insurance.\n                                       </li>\n                                   </ul>\n   \n                                   <ul id=\"hide\">\n                                       <li>\n    <b>THE MARBLELIFE GUARANTEE-</b> \n                                           Subject to limitations set forth under the MARBLELIFE Standard Terms and Conditions, the MARBLELIFE office. identified herein guarantees the\n                                           results of the services performed, and if, within two (2) days after the performance of the services performed, the customer notifies the\n                                           MARBLELIFE office referenced herein of dissatisfaction with the results of the services performed, MARBLELIFE shall reapply the service during\n                                           the next agreed upon service period or at another time mutually agreed upon, or shall refund the relevant portion of the service fee paid by\n                                           customer.\n                                       </li>\n                                   </ul>\n                               </div>\n                           </td>\n                       </tr>\n                   </tbody>\n               </table>' WHERE (`TyepeId` = '241');

