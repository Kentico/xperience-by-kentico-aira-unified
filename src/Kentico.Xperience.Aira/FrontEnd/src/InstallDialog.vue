<template>
    <div id="installFrameManual" class="c-install-modal" style="z-index: 100" hidden>
        <div class="c-install-modal_header">
            <img :src="`${baseUrl}${logoImgRelativePath}`" alt="Kentico" class="c-install-modal_logo">
            <h6 class="mb-0">Install Kentico App</h6>
        </div>
        <div id="installFrameManualMessage" class="c-install-modal_body">
            <p>Install the app on your device.</p>
            <ol>
                <li>Open website in Safari</li>
                <li>
                    Tap on
                    <svg class="c-icon">
                        <use xlink:href=""></use>
                    </svg>
                </li>
                <li>Select “Add to Home Screen”</li>
            </ol>
        </div>
        <div class="c-install-modal_footer">
            <button id="installModalDismissButton-IOS" class="btn btn-outline-info text-uppercase ms-auto">Dismiss</button>
        </div>
    </div>

    <div id="installFrameAndroid" hidden class="c-install-modal" style="z-index: 100">
        <div class="c-install-modal_header">
            <img :src="`${baseUrl}${logoImgRelativePath}`" alt="Kentico" class="c-install-modal_logo">
            <h6 class="mb-0">Install Aira Companion App</h6>
            <button id="installModalDismissButton-android" class="btn btn-outline-info fs-1 text-uppercase mx-auto">Dismiss</button>
        </div>
        <div class="c-install-modal_body">
            <p>Install the app on your device.</p>
        </div>
        <div class="c-install-modal_footer">
            <button class="btn btn-outline-info text-uppercase" id="installButton">Install Kentico App</button>
        </div>
    </div>
</template>

<script>
export default {
    props: {
        baseUrl: String,
        logoImgRelativePath: String
    },
    data() {
        return {
            deferredPrompt: null, // Stores beforeinstallprompt event
        };
    },
    mounted() {
        this.handleInstallPrompt();
        this.handleManualInstall();
        this.setDismissButtons();
    },
    methods: {
        handleInstallPrompt() {
            window.addEventListener('beforeinstallprompt', (e) => {
                e.preventDefault();
                this.deferredPrompt = e;
                this.showAndroidInstallPrompt();
            });

            // If the event already fired before mounting
            if (window.deferredPrompt) {
                this.deferredPrompt = window.deferredPrompt;
                this.showAndroidInstallPrompt();
            }
        },
        showAndroidInstallPrompt() {
            const installButton = document.querySelector('#installButton');
            const androidModal = document.querySelector('#installFrameAndroid');

            if (installButton && androidModal) {
                installButton.addEventListener('click', () => {
                    if (this.deferredPrompt) {
                        this.deferredPrompt.prompt();
                        this.deferredPrompt.userChoice.then(() => {
                            androidModal.setAttribute('hidden', true);
                            this.deferredPrompt = null;
                        });
                    }
                });
                androidModal.removeAttribute('hidden');
            }
        },
        handleManualInstall() {
            if (this.isIOS()) {
                document.getElementById('installFrameManual').removeAttribute('hidden');
            } else if (this.isSamsungBrowser()) {
                document.getElementById('installFrameManualMessage').innerHTML = "For full functionality, use Chrome and approve install prompt";
                document.getElementById('installFrameManual').removeAttribute('hidden');
            }
        },
        setDismissButtons() {
            document.querySelector('#installModalDismissButton-IOS')?.addEventListener('click', () => {
                document.getElementById('installFrameManual')?.setAttribute('hidden', true);
            });

            document.querySelector('#installModalDismissButton-android')?.addEventListener('click', () => {
                document.getElementById('installFrameAndroid')?.setAttribute('hidden', true);
            });
        },
        isSamsungBrowser() {
            return /SamsungBrowser/i.test(navigator.userAgent);
        },
        isIOS() {
            return /iPad|iPhone|iPod/.test(navigator.userAgent);
        }
    }
};
</script>
