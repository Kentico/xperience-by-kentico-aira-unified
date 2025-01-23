import ChatComponent from "./Chat.vue";
import AssetsComponent from "./Assets.vue";
import { createApp } from "vue";

function mountChat(chatElement) {
    const airaBaseUrl = chatElement.dataset.airaBaseUrl;
    const baseUrl = chatElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(chatElement.dataset.navBarModel || "{}");
    const history = JSON.parse(chatElement.dataset.history || {});
    const aiIconUrl = chatElement.dataset.aiIconUrl || "";

    createApp(ChatComponent, {
        airaBaseUrl,
        aiIconUrl,
        baseUrl,
        navBarModel,
        history
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

function mountSignin(){
    const loginButton = document.getElementById('loginButton');
    if (loginButton) {
        loginButton.addEventListener('click', () => {
            const adminLoginUrl = '/admin'; // URL for the login or authentication endpoint
            const popup = window.open(adminLoginUrl, 'Login', 'width=400,height=600');
        
            const closeButton = document.createElement('button');
            closeButton.textContent = 'Close Login Window';
            closeButton.style = `
                position: fixed;
                top: 20px; /* Top-right corner */
                right: 20px;
                z-index: 1000;
                padding: 10px 20px;
                background-color: #ff4d4d; /* Red color */
                color: white;
                border: none;
                border-radius: 5px;
                cursor: pointer;
                font-size: 16px;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            `;
        
            closeButton.onclick = () => {
                if (popup && !popup.closed) {
                    popup.close();
                    closeButton.remove();
                } else {
                    alert('The login window is already closed.');
                }
            };
        
            document.body.appendChild(closeButton);

            const pollTimer = setInterval(() => {
                if (popup.closed) {
                    clearInterval(pollTimer);
                    closeButton.remove();
                    window.location.href = '/aira/chat';
                }
            }, 1000);
        });
    }
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
        mountSignin();
    }
});