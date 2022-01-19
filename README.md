# Introduction 
A program that uses Google Calendar APIs to find what is in your calendar. Then opens any hyperlink stored in the current event.
![image](https://user-images.githubusercontent.com/72658447/150090095-624ada1e-e70a-4942-bc6d-0f78c32b2d91.png)

# Known issues
Program won't start when Arduino isn't attached
The buttons on the right of the clock interface aren't useful. 
API makes a callout every minute, regardless of wether your calendar has changed. (Application is serverless and I'm not sure how to setup a webhook for this purpose.)
