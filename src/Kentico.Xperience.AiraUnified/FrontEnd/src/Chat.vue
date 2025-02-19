<template>
    <div class="c-app_inner">
        <div id="loading" class="c-loading">
            <img :src="`${this.baseUrl}/_content/Kentico.Xperience.AiraUnified/img/spinner.svg`" class="c-loading_spinner" alt="loading spinner" />
        </div>

        <div class="c-app_header">
            <NavBarComponent :airaUnifiedBaseUrl="airaUnifiedBaseUrl" :navBarModel="navBarModel" :baseUrl="baseUrl"/>
        </div>

        <div class="c-app_body">
            <deep-chat v-if="serviceAvailable"
                :avatars="{
                    ai : {
                        src: `${this.baseUrl}${this.aiIconUrl}`,
                        styles: {
                            avatar:
                            {
                                height: '1.75rem',
                                width: '1.75rem'
                            }
                        }
                    },
                    user : {
                        styles: {
                            avatar:
                            {
                                height: '1.75rem',
                                width: '1.75rem'
                            }
                        }
                    }
                }"
                :dropupStyles="{
                    button: {
                        styles: {
                            container: {
                                default: { backgroundColor: '#eff8ff'},
                                hover: { backgroundColor: '#e4f3ff'},
                                click: { backgroundColor: '#d7edff'}
                            }
                        }
                    },
                    menu: {
                        container: {
                            boxShadow: '#e2e2e2 0px 1px 3px 2px'
                        },
                        item: {
                            hover: {
                                backgroundColor: '#e1f2ff'
                            },
                            click: {
                                backgroundColor: '#cfeaff'
                            }
                        },
                        iconContainer: {
                            width: '1.8em'
                        },
                        text: {
                            fontSize: '1.05em'
                        }
                    }
                }"
                :connect="{
                    url: `${this.baseUrl}${this.airaUnifiedBaseUrl}/${this.navBarModel.chatItem.url}/message`,
                    method: 'POST'
                }"
              :chatStyle="{ height: '100%', width: '100%' }"
              :history="[]"
                :textInput="{
                    styles: {
                        container: {
                            borderRadius: '1.5rem',
                            border: '1px solid #8C8C8C',
                            backgroundColor: '#ffffff',
                            boxShadow: 'none',
                            width: '90%',
                        },
                        text: {
                            padding: '.625rem .875rem',
                            fontSize: '.875rem',
                            color: '#231F20',
                            lineHeight: '1.333',
                        },
                            },
                            placeholder: {
                            text: 'Message AIRA' ,
                            style: {
                                color: '#999'
                            }
                        }
                    }"
              :submitButtonStyles="{
                submit: {
                    container: {
                        default: {
                            width: '1.375rem',
                            height: '1.375rem',
                            marginBottom: '0',
                            padding: '.5rem',
                        }
                    },
                    svg: {
                        styles: {
                            default: {
                                width: '1.375rem',
                                height: '1.375rem',
                            }
                        }
                    },
                    loading: {
                        svg: {
                            styles: {
                                default: {
                                    width: '.1875rem',
                                    height: '.1875rem',
                                }
                            }
                        }
                    }
                }
              }"
              id="chatElement"
              ref="chatElementRef"
              :requestBodyLimits="{ maxMessages: 1 }"
              style="border-radius: 8px;"
              :introMessage="{ text: '' }"
              :messageStyles="{
                    default: {
                        shared: { bubble: { fontSize: '0.75rem', lineHeight: '1.375rem', padding: '0.5rem 0.75rem', marginTop: '.375rem' } },
                        ai: { bubble: { background: '#edeeff', borderRadius: '0 1.125rem 1.125rem 1.125rem' } },
                        user: { bubble: { color: '#fff', borderRadius: '1.125rem 1.125rem 0 1.125rem' } }
                    },
                    image: {
                        user: { bubble: { borderRadius: '1rem', overflow: 'clip', textAlign: 'left', display: 'inline-block' } }
                    },
                    html: {
                        shared: {
                            bubble: {
                                backgroundColor: 'unset', 
                                padding: '0px'
                            }
                        }
                    }
                }">
            </deep-chat>
            <div class="c-empty-page-layout justify-content-start" v-if="!serviceAvailable">
                <div class="d-flex flex-wrap justify-content-center gap-3 text-center">
                    <svg class="c-image service-unavailable" width="150" height="189" viewBox="0 0 150 189" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <g clip-path="url(#clip0_844_34259)">
                            <g clip-path="url(#clip1_844_34259)">
                                <path d="M46.7024 68.3112C44.8611 66.516 41.9132 66.5533 40.118 68.3946C38.3228 70.2358 38.3601 73.1837 40.2013 74.979L68.3623 102.436L40.2013 129.893C38.3601 131.689 38.3228 134.637 40.118 136.478C41.9132 138.319 44.8611 138.356 46.7024 136.561L75.0322 108.939L103.361 136.561C105.203 138.356 108.151 138.319 109.946 136.477C111.741 134.636 111.704 131.688 109.863 129.893L81.7021 102.436L109.863 74.9795C111.704 73.1842 111.741 70.2363 109.946 68.3951C108.151 66.5538 105.203 66.5165 103.361 68.3117L75.0322 95.933L46.7024 68.3112Z" fill="#FF8852"/>
                                <path fill-rule="evenodd" clip-rule="evenodd" d="M22.0883 49.3125C15.3474 49.3125 9.81273 54.8471 9.81273 61.5881L9.8125 139.595C9.8125 146.307 15.3186 151.791 22.088 151.791H42.3854C44.9569 151.791 47.0416 153.876 47.0416 156.447V174.234L71.9733 152.909C72.8167 152.187 73.89 151.791 74.9999 151.791H127.912C134.681 151.791 140.187 146.307 140.187 139.595V61.5881C140.187 54.8471 134.653 49.3125 127.912 49.3125H22.0883ZM0.50023 61.5881C0.50023 49.704 10.2042 40 22.0883 40H127.912C139.796 40 149.5 49.704 149.5 61.5881V139.595C149.5 151.507 139.767 161.103 127.912 161.103H76.7196L45.4119 187.882C44.0307 189.064 42.0884 189.333 40.4375 188.573C38.7867 187.813 37.7291 186.161 37.7291 184.344V161.103H22.088C10.2325 161.103 0.5 151.507 0.5 139.595L0.50023 61.5881Z" fill="#FF8852"/>
                            </g>
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M139.738 10.6021L148.592 13.5533C149.803 13.9571 149.803 15.6703 148.592 16.074L139.738 19.0252C139.341 19.1574 139.03 19.4687 138.898 19.8654L135.947 28.7189C135.543 29.9301 133.83 29.9301 133.426 28.7189L130.475 19.8654C130.343 19.4687 130.031 19.1574 129.635 19.0252L120.781 16.074C119.57 15.6703 119.57 13.9571 120.781 13.5533L129.635 10.6021C130.031 10.4699 130.343 10.1586 130.475 9.76191L133.426 0.908415C133.83 -0.302802 135.543 -0.302806 135.947 0.908411L138.898 9.76191C139.03 10.1586 139.341 10.4699 139.738 10.6021ZM138.618 13.9631L141.169 14.8137L138.618 15.6642C137.163 16.1491 136.022 17.2905 135.537 18.7451L134.686 21.2968L133.836 18.7451C133.351 17.2905 132.209 16.1491 130.755 15.6642L128.203 14.8137L130.755 13.9631C132.209 13.4782 133.351 12.3368 133.836 10.8822L134.686 8.3305L135.537 10.8822C136.022 12.3368 137.163 13.4782 138.618 13.9631Z" fill="black"/>
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M109.892 27.1831C109.496 27.0508 109.184 26.7395 109.052 26.3428L104.996 14.1732C104.592 12.9619 102.879 12.9619 102.475 14.1732L98.4183 26.3428C98.2861 26.7395 97.9748 27.0508 97.5781 27.1831L85.4084 31.2396C84.1972 31.6434 84.1972 33.3566 85.4084 33.7604L97.5781 37.8169C97.9748 37.9492 98.2861 38.2605 98.4183 38.6572L102.475 50.8268C102.879 52.0381 104.592 52.0381 104.996 50.8269L109.052 38.6572C109.184 38.2605 109.496 37.9492 109.892 37.8169L122.062 33.7604C123.273 33.3566 123.273 31.6434 122.062 31.2396L109.892 27.1831ZM114.64 32.5L108.772 30.544C107.318 30.0592 106.176 28.9177 105.691 27.4632L103.735 21.5952L101.779 27.4632C101.294 28.9177 100.153 30.0592 98.6984 30.544L92.8305 32.5L98.6984 34.456C100.153 34.9408 101.294 36.0822 101.779 37.5368L103.735 43.4048L105.691 37.5368C106.176 36.0822 107.318 34.9408 108.772 34.456L114.64 32.5Z" fill="black"/>
                            <path d="M121.897 51.5018C122.294 51.3695 122.605 51.0583 122.737 50.6615L124.583 45.1242C124.987 43.913 126.7 43.913 127.104 45.1242L128.949 50.6615C129.082 51.0583 129.393 51.3695 129.79 51.5018L135.327 53.3476C136.538 53.7513 136.538 55.4645 135.327 55.8683L129.79 57.714C129.393 57.8463 129.082 58.1576 128.949 58.5543L127.104 64.0916C126.7 65.3028 124.987 65.3028 124.583 64.0916L122.737 58.5543C122.605 58.1576 122.294 57.8463 121.897 57.714L116.359 55.8683C115.148 55.4645 115.148 53.7513 116.359 53.3476L121.897 51.5018Z" fill="black"/>
                        </g>
                        <defs>
                            <clipPath id="clip0_844_34259">
                                <rect width="149" height="189" fill="white" transform="translate(0.5)"/>
                            </clipPath>
                            <clipPath id="clip1_844_34259">
                                <rect width="149" height="149" fill="white" transform="translate(0.5 40)"/>
                            </clipPath>
                        </defs>
                    </svg>
                    <p class="mt-5"><strong>{{`${this.servicePageModel.chatUnvailableMainMessage}`}}</strong></p>
                    <p class="fs-2">{{`${this.servicePageModel.chatUnavailableTryAgainMessage}`}}</p>
                </div>
            </div>
          <InstallDialogComponent v-if="!isInstalledPWA" :baseUrl="baseUrl" :logoImgRelativePath="navBarModel.logoImgRelativePath" />
        </div>
    </div>
</template>

<script>
import 'deep-chat';
import NavBarComponent from "./Navigation.vue";
import InstallDialogComponent from './InstallDialog.vue';

export default {
    components: {
        NavBarComponent,
        InstallDialogComponent
    },
    props: {
        airaUnifiedBaseUrl: null,
        aiIconUrl: null,
        baseUrl: null,
        usePromptUrl: null,
        navBarModel: null,
        rawHistory: null,
        servicePageModel: null
    },
    data() {
        return {
            themeColor: "#8107c1",
            themeColorInRgb: "rgb(129, 7, 193)",
            submitButton: null,
            started: false,
            messagesMetadata: new Map(),
            history: [],
            showAllSuggestions: false,
            isInstalledPWA: false,
            serviceAvailable: true
        }
    },
    mounted() {
        document.onreadystatechange = () => {
            if (document.readyState === "complete") {
                this.main();

                this.isInstalledPWA = window.matchMedia('(display-mode: window-controls-overlay)').matches ||
                    window.matchMedia('(display-mode: standalone)').matches;
            }
        };
    },
    methods: {
        main() {
            setTimeout(() => {
                var modal = document.querySelector('#loading');
                if (modal) {
                    modal.classList.add('is-hidden');
                }
                setTimeout(function () {
                    modal.parentNode.removeChild(modal);
                }, 500);
            }, 1000);

            this.$refs.chatElementRef.onComponentRender = async () => {
                this.setBorders();

                if (!this.started) {
                    this.started = true;
                    document.addEventListener('visibilitychange', function () {
                        if (document.visibilityState === 'visible' && this.$refs.chatElementRef) {
                            this.$refs.chatElementRef.scrollToBottom();
                        }
                    });

                    this.setRequestInterceptor();
                    this.setOnMessage();
                    this.setOnError();
                    this.setResponseInterceptor();
                    this.setHistory();
                }

                const newSubmitButton = this.$refs.chatElementRef.shadowRoot.querySelector('.input-button');
                if (this.submitButton !== newSubmitButton) {
                    this.submitButton = newSubmitButton;
                    this.addClassesToShadowRoot();
                }

                this.bindPromptButtons();
            };
        },
        typeIntoInput(inputElement, text) {
            inputElement.focus();
            inputElement.innerHTML  = "";

            for (let char of text) {
                inputElement.innerHTML  += char;
                inputElement.dispatchEvent(new Event("input", { bubbles: true }));
            }
        },
        bindPromptButtons() {
            this.$refs.chatElementRef.shadowRoot.querySelectorAll('button[prompt-quick-suggestion-button]').forEach(button => {
                button.addEventListener('click', async () => {
                    const text = button.value.valueOf();

                    const buttonGroupId = button.parentNode.getAttribute("prompt-quick-suggestion-button-group-id");

                    this.history = this.history.filter(x => (x.promptQuickSuggestionGroupId === undefined) || x.promptQuickSuggestionGroupId.toString() !== buttonGroupId);
                    this.$refs.chatElementRef.clearMessages(true);

                    this.history.forEach(x => {
                        this.$refs.chatElementRef.addMessage(x);
                    });

                    this.bindPromptButtons();

                    setTimeout(() => {
                        const textInput = this.$refs.chatElementRef.shadowRoot.getElementById("text-input");
                        textInput.classList.remove("text-input-placeholder");

                        this.typeIntoInput(textInput, text);
                    }, 50);

                    const sendUsePromptUrl = `${this.baseUrl}${this.airaUnifiedBaseUrl}/${this.usePromptUrl}`;
                    await this.removeUsedPromptGroup(buttonGroupId, sendUsePromptUrl);
                });
            });
        },
        async removeUsedPromptGroup(groupId, sendUsePromptUrl) {
            try {
                await fetch(sendUsePromptUrl, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        groupId: groupId,
                    }),
                });
            }
            catch (error) {
                console.error('An error occurred:', error.message);
            }
        },
        setRequestInterceptor() {
            this.$refs.chatElementRef.requestInterceptor = async (requestDetails) => {
                const formData = new FormData();

                this.history.push(requestDetails.body.messages[0]);

                let jsonData = "";

                if (Object.hasOwn(requestDetails.body, 'messages'))
                { 
                    jsonData = requestDetails.body.messages[0].text;
                }
                else
                {
                    const entries = requestDetails.body.entries();
                    if (entries !== null)
                    {
                        let hasMessages = false;
                        for (const [key, value] of requestDetails.body.entries()) {
                            if (key === 'message1')
                            {
                                let parsedValue;
                                try {
                                    parsedValue = JSON.parse(value);
                                } catch (e) {
                                    parsedValue = value;
                                }
                                if (parsedValue && parsedValue.text) {
                                    formData.append('message', parsedValue.text);
                                    hasMessages = true;
                                }
                            }
                            else if (key === 'files') {
                                formData.append(key, value);
                            }
                        }

                        if (!hasMessages) {
                            formData.append('messages', "");
                        }
                    }
                }

                const modifiedRequestDetails = {
                    ...requestDetails,
                    body: jsonData ?? formData,
                    headers: {
                        ...requestDetails.headers
                    },
                };

                return modifiedRequestDetails;
            };
        },
        setResponseInterceptor() {
            this.$refs.chatElementRef.responseInterceptor = (response) => {
                const messageViewModel = this.getMessageViewModel(response);
                
                this.history.push(messageViewModel);

                if (response.serviceUnavailable)
                {
                    this.serviceAvailable = false
                }

                if (response.quickPrompts && response.quickPrompts.length > 0)
                {
                    this.$refs.chatElementRef.addMessage(messageViewModel);
                    const promptMessage = this.getPromptsViewModel(response);
                    
                    this.history.push(promptMessage);

                    return promptMessage;
                }
                return messageViewModel;
            };
        },
        setOnMessage() {
            this.$refs.chatElementRef.onMessage = (message) => {
                this.bindPromptButtons();
            };
        },
        setOnError() {
            this.$refs.chatElementRef.onError = (error) => {
                this.serviceAvailable = false;
            }
        },
        setBorders(){
            this.$refs.chatElementRef.style.borderLeftStyle = 'none';
            this.$refs.chatElementRef.style.borderTopStyle = 'none';
            this.$refs.chatElementRef.style.borderRightStyle = 'none';
            this.$refs.chatElementRef.style.borderBottomStyle = 'none';
        },
        addClassesToShadowRoot() {
            let shadowRoot = this.$refs.chatElementRef.shadowRoot;

            const style = document.createElement('style');

            style.textContent =
                `
                #messages {
                    scrollbar-width: none;
                }
                #messages::-webkit-scrollbar {
                    display: none;
                }
                .c-prompt-btn{
                  appearance: none;
                  background: #fff;
                  cursor: pointer;
                  font-size: .875rem;
                  line-height: 1rem;
                  padding: .75rem;
                  text-align: center;
                  color: #000D48;
                  border: 2px solid #000D48;
                  border-radius: .375rem;
                  transition: background-color 0.2s ease;
                }
                .c-prompt-btn:hover{
                  background: #ebe7e5;
                }
                .c-prompt-btn:active{
                  background: #ddd9d7;
                }

                .c-prompt-btn-wrapper{
                  display: flex;
                  flex-wrap: wrap;
                  align-items: center;
                  gap: .25rem;
                  padding-top: .25rem;
                }

                .btn-outline-primary {
                    color: ${this.themeColorInRgb};
                    background-color: transparent;
                    border: 1px solid ${this.themeColorInRgb};
                    font-size: 0.75rem;
                    border-radius: 24px;
                    padding: 6px 12px;
                    margin: 4px;
                    transition: color 0.3s ease, background-color 0.3s ease, border-color 0.3s ease;
                    display: inline-block;
                }

                .user-message-text {
                    background-color: ${this.themeColor} !important;
                }

                .btn-outline-primary:hover {
                    color: #fff;
                    background-color: ${this.themeColorInRgb};
                    border-color: ${this.themeColorInRgb};
                }

                .btn-outline-primary:active {
                    color: #fff;
                    background-color: #C64300;
                    border-color: #C64300;
                }

                .btn-outline-primary:disabled {
                        color: ${this.themeColorInRgb};
                    background-color: transparent;
                        border-color: ${this.themeColorInRgb};
                    opacity: 0.65;
                    pointer-events: none;
                }

                .container {
                    display: flex;
                    flex-direction: row;
                    align-items: flex-start;
                    flex-wrap: wrap;
                }

                .message-bubble div {
                    margin-bottom: 8px;
                }

                .message-bubble .btn-outline-primary {
                    margin-right: 8px;
                }
                .message-bubble .deep-chat-button {
                    margin-right: 8px;
                }

                .lds-ring, .lds-ring div {
                    box-sizing: border-box;
                }

                .lds-ring {
                    display: inline-block;
                    position: relative;
                    width: 80px;
                    height: 80px;
                    margin-bottom: 0px;
                }

                .lds-ring div {
                    box-sizing: border-box;
                    display: block;
                    position: absolute;
                    width: 64px;
                    height: 64px;
                    margin: 8px;
                    border: 5px solid currentColor;
                    border-radius: 50%;
                    animation: lds-ring 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
                    border-color: currentColor transparent transparent transparent;
                }

                .lds-ring div:nth-child(1) {
                    animation-delay: -0.45s;
                }

                .lds-ring div:nth-child(2) {
                    animation-delay: -0.3s;
                }

                .lds-ring div:nth-child(3) {
                    animation-delay: -0.15s;
                }

                @@keyframes lds-ring {
                    0% {
                        transform: rotate(0deg);
                    }

                    100% {
                        transform: rotate(360deg);
                    }
                }`
            shadowRoot.appendChild(style);
        },
        setHistory() {
            for (const x of this.rawHistory) {
                const messageViewModel = this.getMessageViewModel(x);
                
                this.history.push(messageViewModel);
                this.$refs.chatElementRef.history.push(messageViewModel);
                this.$refs.chatElementRef.addMessage(messageViewModel);

                if (x.quickPrompts.length > 0)
                {
                    const promptMessage = this.getPromptsViewModel(x);
                    this.history.push(promptMessage);
                    this.$refs.chatElementRef.history.push(promptMessage);
                    this.$refs.chatElementRef.addMessage(promptMessage);
                }
            }
        },
        getPromptsViewModel(message) {
            let prompts = `<div class="c-prompt-btn-wrapper" prompt-quick-suggestion-button-group-id="${message.quickPromptsGroupId}">`;

            for (var prompt of message.quickPrompts) {
                prompts += `<button class="c-prompt-btn" prompt-quick-suggestion-button value="${prompt}">${prompt}</button>`;
            }

            prompts += '</div>';

            return {
                role: 'ai',
                html: prompts,
                promptQuickSuggestionGroupId: `${message.quickPromptsGroupId}`
            }
        },
        getMessageViewModel(message) {
            return {
                role: message.role ?? "",
                text: message.message ?? ""
            }
        },
        isJSONWithProperty(string, property) {
            try {
                const json = JSON.parse(string);
                return json && typeof json === 'object' && property in json;
            } catch (e) {
                return false;
            }
        }
    }
};
</script>