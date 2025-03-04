# Usage Guide

This library enables the use of the Aira Unified chat and smart uploader, serving as an alternative admin user interface optimized for smartphones.

## Accessing the Aira Unified PWA (Progressive Web App)

After successfully configuring the Aira Unified application in the Xperience admin UI and assigning the required roles to the appropriate users, as detailed in the [README](../README.md), those users can access the application on their smartphones.

The configured URL will be relative to the base URL of the application. For example, if your base URL is `https://localhost:34886` and you have set the `Relative Path Base` to `/aira`, the Aira application will be available at `https://localhost:34886/aira/signin`.

After visiting that URL on a smartphone, users will see an installation dialog explaining the steps required to install the app as a PWA on their home screen. The process varies between smartphone operating systems. After completing one of the steps below, the application will appear on the [home screen](#aira-unified-on-home-screen).

1. **Android users**  
   Android supports PWA installation via a button. Therefore, Android users will be presented with an option to install the application. Once confirmed, the app will be added to their home screen.

   ![Android installation](/images/AiraUnifiedInstallationDialogAndroid.png)

2. **iOS users**  
   iOS does not support direct installation via a button. Instead, users will be guided on how to add the PWA to their home screen. This involves tapping the share button at the bottom of the Safari browser and selecting "Add to Home Screen."

   ![iOS installation](/images/AiraUnifiedInstallationDialogIOS.jpg)  
   ![Add to home screen](/images/AiraUnifiedAddToHomeScreen.jpg)  
   ![Create the PWA](/images/AiraUnifiedCreatePWA.jpg)  

### Aira Unified on Home Screen
![Aira Unified on home screen](/images/AiraUnifiedOnHomeScreen.jpg)

## Using the Chatbot

After opening the PWA, if a user is not signed in yet, the admin application users with a role that includes **View** permission for the Aira Unified application can sign in by clicking the **Continue with XbyK** button, which redirects them to the Xperience native administration sign-in page within the PWA.

Users are expected to sign in and close the administration interface, as explained at the top of the screen.  
![Sign in](/images/AiraUnifiedSignIn.jpg)

Aira Unified remembers the signed in user and the PWA will not require to repeat the sign in process.

After returning to the application, users will be redirected to the chat page.
They can interact with Aira, which is configured with knowledge about Xperience, allowing it to answer questions about the product and explain various functionalities.

Users can inquire about content from the admin site, and the AI will provide real-time responses.
![Chat](/images/AiraUnifiedChat.jpg)

## Using the Smart Uploader

Admin application users with a role that includes **Create** permission for the Aira Unified application can access the Smart Uploader.

It can be opened by tapping the menu button and navigating to the **Smart Upload** page.  
![Menu](/images/AiraUnifiedNavigation.jpg)  
![Smart uploader](/images/SmartAssetUploader.jpg)
