<template>
    <div id="installFrameManual" class="c-install-modal" style="z-index: 100" hidden>
        <div class="c-install-modal_header">
            <img :src="`${this.baseUrl}${this.logoImgRelativePath}`" alt="Kentico" class="c-install-modal_logo">
            <h6 class="mb-0">Install Kentico App</h6>
        </div>
        <div id="installFrameManualMessage" class="c-install-modal_body">
            <p>Install the app on your device.</p>
            <ol>
                <li>Open website in Safari</li>
                <li>
                    Tap on
                    <svg class="c-icon ">
                        <use xlink:href=""></use>
                    </svg>
                </li>
                <li>Select “Add to Home Screen”</li>
            </ol>
        </div>
        <div class="c-install-modal_footer">
            <button id="installModalDismissButton-IOS" class="btn btn-outline-info text-uppercase ms-auto">dismiss</button>
        </div>
    </div>

    <div id="installFrameAndroid" hidden class="c-install-modal" style="z-index: 100">
        <div class="c-install-modal_header">
            <img :src="`${this.baseUrl}${this.logoImgRelativePath}`"  alt="Kentico" class="c-install-modal_logo">
            <h6 class="mb-0">Install Aira Companion App</h6>
            <button id="installModalDismissButton-android" style="letter-spacing: 0.025em" class="btn btn-outline-info btn-outline-info-dismiss fs-1 text-uppercase mx-auto">dismiss</button>
        </div>
        <div class="c-install-modal_body">
            <p>Install the app on your device.</p>
        </div>
        <br />
        <div class="c-install-modal_footer">
            <button class="btn btn-outline-info text-uppercase" id="installButton">Install Kentico App</button>
        </div>
    </div>
</template>

<script>
export default {
    props: {
        baseUrl: null,
        logoImgRelativePath: null
    },
    data() {
        return {
        }
    },
    mounted() {
        this.main();
        this.setDismissButtons();
    },
    methods: {
        main() {
            console.log(this.baseUrl);
            console.log(this.logoImgRelativePath);
            if (!this.isIOS())
            {
                window.addEventListener('beforeinstallprompt', e => {
                    e.preventDefault();
                    document.querySelector('#installButton').addEventListener('click', () => {
                        e.prompt();
                        document.querySelector('#installFrameAndroid').setAttribute('hidden', true);
                    });

                    document.querySelector('#installFrameAndroid').removeAttribute('hidden');
                });
            }

            if (this.isIOS()) {
                document.getElementById('installFrameManual').removeAttribute('hidden');
            }
            if (this.isSamsungBrowser()) {
                document.getElementById('installFrameManualMessage').innerHTML = "For full functionality, use Chrome and approve install prompt";
                document.getElementById('installFrameManual').removeAttribute('hidden');
            }
        },
        isSamsungBrowser () {
            return /SamsungBrowser/i.test(navigator.userAgent);
        },
        isIOS(){
            return /iPad|iPhone|iPod/.test(navigator.userAgent);
        },
        setDismissButtons() {
            document.querySelector('#installModalDismissButton-IOS').addEventListener('click', function () {
                var modal = document.querySelector('#installFrameManual');
                if (modal) {
                    modal.setAttribute('hidden', true);
                }
            });

            document.querySelector('#installModalDismissButton-android').addEventListener('click', function () {
                var modal = document.querySelector('#installFrameAndroid');
                if (modal) {
                    modal.setAttribute('hidden', true);
                }
            });
        }
    }
};
</script>