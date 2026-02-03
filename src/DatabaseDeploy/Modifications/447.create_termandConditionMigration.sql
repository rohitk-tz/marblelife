USE `makalu`;
DROP procedure IF EXISTS `migration_termAndCondtion`;

DELIMITER $$
USE `makalu`$$
CREATE PROCEDURE `migration_termAndCondtion` ()
BEGIN
declare _organizationId int;

create table distinctIds
select distinct Id from  makalu.franchisee;
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
select id into _organizationId from distinctIds Limit 1;

INSERT INTO `termsAndConditionFranchisee` ( `FranchiseeId`, `TyepeId`,`TermAndCondition`,`IsDeleted`) VALUES (_organizationId, '240','<table style="width:100%;" cellpadding="0" cellspacing="0">
               
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

INSERT INTO `termsAndConditionFranchisee` ( `FranchiseeId`, `TyepeId`,`TermAndCondition`,`IsDeleted`) VALUES (_organizationId, '241','<style>
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
SET SQL_SAFE_UPDATES = 0;
DELETE FROM distinctIds WHERE  id = _organizationId;
SET SQL_SAFE_UPDATES = 1;
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;


call migration_termAndCondtion();