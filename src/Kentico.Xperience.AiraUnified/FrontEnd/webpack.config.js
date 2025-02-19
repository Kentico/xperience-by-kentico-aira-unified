import TerserPlugin from "terser-webpack-plugin";
import { resolve as _resolve, dirname } from "path";
import { fileURLToPath } from "url";
import { VueLoaderPlugin } from "vue-loader";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const isProduction = process.env.NODE_ENV === "production";

const config = {
    entry: "./src/main.js",
    output: {
        path: _resolve(__dirname, "../wwwroot/js"),
        filename: "index.js",
        module: true,
    },
    experiments: {
        outputModule: true,
    },

    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: "vue-loader",
            },
            {
                test: /\.(js|jsx)$/i,
                loader: "babel-loader",
            },
            {
                test: /\.(eot|svg|ttf|woff|woff2|png|jpg|gif)$/i,
                type: "asset",
            },
        ],
    },
    plugins: [new VueLoaderPlugin()],
    optimization: {
        minimize: true,
        minimizer: [new TerserPlugin()],
    },
    resolve: {
        extensions: [".js", ".vue"],
    },
};

export default () => {
    if (isProduction) {
        config.mode = "production";
    } else {
        config.mode = "development";
    }
    return config;
};
