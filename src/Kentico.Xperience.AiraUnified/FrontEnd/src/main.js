import ChatComponent from "./Chat.vue";
import AssetsComponent from "./Assets.vue";
import InstallDialogComponent from "./InstallDialog.vue";
import { createApp } from "vue";

function mountChat(chatElement) {
    const airaUnifiedBaseUrl = chatElement.dataset.airaUnifiedBaseUrl;
    const baseUrl = chatElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(chatElement.dataset.navBarModel || "{}");
    const rawHistory = JSON.parse(chatElement.dataset.history || {});
    const aiIconUrl = chatElement.dataset.aiIconUrl || "";
    const usePromptUrl = chatElement.dataset.usePromptUrl || "";
    const servicePageModel = JSON.parse(chatElement.dataset.servicePageModel || {});

    createApp(ChatComponent, {
        airaUnifiedBaseUrl,
        aiIconUrl,
        baseUrl,
        usePromptUrl,
        navBarModel,
        rawHistory,
        servicePageModel
    }).mount("#chat-app");
}

function mountAssets(assetsElement) {
    const airaUnifiedBaseUrl = assetsElement.dataset.airaUnifiedBaseUrl;
    const baseUrl = assetsElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(assetsElement.dataset.navBarModel || "{}");
    const allowedFileExtensionsUrl = assetsElement.dataset.allowedFileExtensionsUrl || "";
    const selectFilesButton = assetsElement.dataset.selectFilesButton || "";
    const uploadSuccessfulMessage = assetsElement.dataset.uploadSuccessfulMessage || "";
    
    createApp(AssetsComponent, {
        airaUnifiedBaseUrl,
        baseUrl,
        navBarModel,
        allowedFileExtensionsUrl,
        selectFilesButton,
        uploadSuccessfulMessage,
    }).mount("#assets-app");
}

function mountSignin(signinElement) {
    const loginButton = document.getElementById('loginButton');
    if (loginButton) {
        loginButton.addEventListener('click', () => {
            openModalLogin(signinElement);
        });
    }

    const isInstalledPWA = window.matchMedia('(display-mode: window-controls-overlay)').matches ||
        window.matchMedia('(display-mode: standalone)').matches;
    
    if (!isInstalledPWA)
    {
        const baseUrl = signinElement.dataset.baseUrl || "";
        const logoImgRelativePath = signinElement.dataset.logoImgRelativePath || "";

        createApp(InstallDialogComponent, {
            baseUrl,
            logoImgRelativePath
        }).mount("#install-dialog");
    }
}

function openModalLogin(signinElement) {
    const baseUrl = signinElement.dataset.baseUrl || "";
    const airaUnifiedBaseUrl = signinElement.dataset.airaUnifiedBaseUrl || "";
    const chatUrl = signinElement.dataset.chatUrl || "";

    const body = document.getElementsByClassName('c-app_body')[0];

    const modalOverlay = document.createElement('div');
    modalOverlay.style = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, 0.5);
        z-index: 1000;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
    `;

    const iframe = document.createElement('iframe');
    iframe.src = '/admin';
    iframe.style = `
        width: 100%;
        height: 100%;
        background: white;
        border: none;
        position: absolute;
        top: 0;
        left: 0;
        z-index: 1000;
    `;

    modalOverlay.appendChild(iframe);
    body.appendChild(modalOverlay);
    showReturnButton();

    function showReturnButton() {
        const messageBox = document.createElement('div');
        messageBox.innerText = "After Signing in, press the return button to return to the app.";
        messageBox.style = `
            position: absolute;
            top: 0%;
            left: 0%;
            width: 40%;
            background: rgba(0, 0, 0, 0.7);
            color: white;
            padding: 1vw 2vw;
            border-radius: 8px;
            font-size: min(4vw, 16px);
            text-align: center;
            z-index: 1001;
            white-space: normal;
            word-wrap: break-word;
            display: flex;
            justify-content: center;
            align-items: center;
        `;

        const closeButton = document.createElement('button');
        closeButton.innerText = "Return to App";
        closeButton.style = `
            position: absolute;
            top: 0%;
            right: 0%;
            width: 15%;
            padding: 1vw 2vw;
            background-color: #FF4D4F;
            color: white;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: min(4vw, 16px);
            z-index: 1002;
            white-space: normal;
            word-wrap: break-word;
            display: flex;
            justify-content: center;
            align-items: center;
        `;

        messageBox.style.maxWidth = "40vw";
        closeButton.style.maxWidth = "20vw";

        closeButton.onclick = () => {
            modalOverlay.remove();
            window.location.href = `${baseUrl}${airaUnifiedBaseUrl}/${chatUrl}`;
        };

        modalOverlay.appendChild(messageBox);
        modalOverlay.appendChild(closeButton);
    }
}



document.addEventListener('DOMContentLoaded', () => {
    const chatElement = document.getElementById("chat-app");
    const assetsElement = document.getElementById("assets-app");
    const signinElement = document.getElementById("aira-unified-kentico-admin-signin");

    if (chatElement){
        mountChat(chatElement);
    }
    else if (assetsElement){
        mountAssets(assetsElement);
    }
    else if (signinElement){
        mountSignin(signinElement);
    }
});