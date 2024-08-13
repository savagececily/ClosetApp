import React from 'react';
import ReactDOM from "react-dom/client";
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import reportWebVitals from './reportWebVitals';
import { initializeIcons, ThemeProvider, PartialTheme } from '@fluentui/react';

initializeIcons(/* optional base url */);

const appTheme: PartialTheme = {
    palette: {
        themePrimary: '#006080',
        themeLighterAlt: '#f0f7fa',
        themeLighter: '#c5e1eb',
        themeLight: '#98c8d9',
        themeTertiary: '#4798b3',
        themeSecondary: '#116f8f',
        themeDarkAlt: '#005673',
        themeDark: '#004961',
        themeDarker: '#003647',
        neutralLighterAlt: '#faf9f8',
        neutralLighter: '#f3f2f1',
        neutralLight: '#edebe9',
        neutralQuaternaryAlt: '#e1dfdd',
        neutralQuaternary: '#d0d0d0',
        neutralTertiaryAlt: '#c8c6c4',
        neutralTertiary: '#a19f9d',
        neutralSecondary: '#605e5c',
        neutralPrimaryAlt: '#3b3a39',
        neutralPrimary: '#323130',
        neutralDark: '#201f1e',
        black: '#000000',
        white: '#ffffff',
    }
};

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
    <React.StrictMode>
        <BrowserRouter>
            <ThemeProvider theme={appTheme}>
                <App />
            </ThemeProvider>
        </BrowserRouter>

    </React.StrictMode>
);

serviceWorkerRegistration.unregister();

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
