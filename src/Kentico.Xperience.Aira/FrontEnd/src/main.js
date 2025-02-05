import ChatComponent from "./Chat.vue";
import AssetsComponent from "./Assets.vue";
import { createApp } from "vue";

function mountChat(chatElement) {
    const airaBaseUrl = chatElement.dataset.airaBaseUrl;
    const baseUrl = chatElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(chatElement.dataset.navBarModel || "{}");
    const rawHistory = JSON.parse(chatElement.dataset.history || {});
    const aiIconUrl = chatElement.dataset.aiIconUrl || "";
    const usePromptUrl = chatElement.dataset.usePromptUrl || "";

    createApp(ChatComponent, {
        airaBaseUrl,
        aiIconUrl,
        baseUrl,
        usePromptUrl,
        navBarModel,
        rawHistory
    }).mount("#chat-app");
}

function mountAssets(assetsElement) {
    const airaBaseUrl = assetsElement.dataset.airaBaseUrl;
    const baseUrl = assetsElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(assetsElement.dataset.navBarModel || "{}");
    
    createApp(AssetsComponent, {
        airaBaseUrl,
        baseUrl,
        navBarModel
    }).mount("#assets-app");
}

function mountSignin(signinElement) {
    const loginButton = document.getElementById('loginButton');
    if (loginButton) {
        loginButton.addEventListener('click', () => {
            openModalLogin(signinElement);
        });
    }
}

function openModalLogin(signinElement) {
    const baseUrl = signinElement.dataset.baseUrl || "";
    const airaBaseUrl = signinElement.dataset.airaBaseUrl || "";
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
    `;

    const iframe = document.createElement('iframe');
    iframe.src = '/admin'; // URL for the admin page
    iframe.style = `
        width: 90%;
        height: 90%;
        background: white;
        border: none;
        border-radius: 8px;
        box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
    `;

    const closeButton = document.createElement('button');

    const closeImg = document.createElement('img');
    closeImg.classList.add('c-icon');
    closeImg.classList.add('fs-6');
    closeImg.src = `${baseUrl}/_content/Kentico.Xperience.Aira/img/icons/cross.svg`;
    closeImg.innerHTML = 'X';

    closeButton.style = `
        position: fixed;
        top: 5%;
        right: 5%;
        z-index: 1001;
        padding: 10px 15px;
        background-color: #FF4D4F;
        color: white;
        border: none;
        border-radius: 5px;
        cursor: pointer;
        font-size: 17px;
    `;
    closeButton.classList.add('btn');
    closeButton.classList.add('btn-remove-img');
    
    closeButton.appendChild(closeImg);

    closeButton.onclick = () => {
        modalOverlay.remove();
        window.location.href = `${baseUrl}${airaBaseUrl}/${chatUrl}`;
    }

    modalOverlay.appendChild(iframe);
    modalOverlay.appendChild(closeButton);
    body.appendChild(modalOverlay);
}

document.addEventListener('DOMContentLoaded', () => {
    const chatElement = document.getElementById("chat-app");
    const assetsElement = document.getElementById("assets-app");
    const signinElement = document.getElementById("aira-kentico-admin-signin");

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