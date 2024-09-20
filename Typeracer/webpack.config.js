const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    entry: './src/index.js',  // React entry point
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),
        filename: 'bundle.js',  // Output file
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                },
            },
            {
                test: /\.css$/,  // Files with .css extension
                use: ['style-loader', 'css-loader'],  // Loaders to use
            },
        ],
    },
    resolve: {
        extensions: ['.js', '.jsx'],
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: './src/index.html',
            filename: './index.html',
        }),
    ],
};
