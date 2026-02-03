How to deploy?

1. Change Project Configuration acc. to the environment where you want to deploy. For test it is "taazaa_qa", and for Production it is "production".

2. Publish Web API directly from Visual Studio. Choose Profile "WebDeploy_QA" for deployment to test, and Choose "Production" for deployment to production.

3. Build Jobs, make sure configuration was appropriately set. 

4. Open Command Prompt, and Change Directory to "C:\Program Files\IIS\Microsoft Web Deploy V3" , here the utility for MSDeploy is located

BEFORE YOU DEPLOY THE JOBS YOU NEED TO MAKE SURE THIS IS APPROPRIATELY STOPPED FIRST ON THE SERVER
5. Execute the command as specified in deploy.txt. Make sure source-contentPath is accurately set.

6. Repeat the same thing from step 3 to step 6 for DatabaseDeploy
BEFORE YOU DEPLOY THE DATABASE DEPLOY YOU NEED TO BACKUP THE EXISTING DATABASE INSTANCE

7. For deploying Web.UI, 
	open the git bash, 
	go to the Web.Ui directory
	run => npm install
	run => bower install
	For Production, run => gulp production
	For test, run => gulp test
	
8. Use the msdeploy command for Web Ui from deploy.txt. Make sure source-contentPath is accurately set.



---

9. Open the server
10. Go To Directory where DatabaseDeploy was published, check the config file so the app settings key for "RecreateDB" should be set to False
11. Execute the exe

12. Start the Job from Services.
	

