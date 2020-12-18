// "Production" enabled environment

export const environment = {
    production: true,
    hmr: false,
    appConfig: 'appconfig.production.json',
    taxPercent: 7,
    defaultUom: 'Month',
    defaultStatus: 'Created',
    emsPdfApiUrl: 'https://emspdfapi00.azurewebsites.net/api/',
    quoteCreatePdfEndPoint: 'pdfmake/quotecreate',
    estimteCreatePdfEndPoint: 'pdfmake/estimatecreate',
    invoiceCreatePdfEndPoint: 'pdfmake/invoicecreate',
    //pdfEnvironment: "Azure-Dev"
    //pdfEnvironment: "Azure-Prd-Quiptrix"
    pdfEnvironment: "Azure-Prd-PondusOps"
};
