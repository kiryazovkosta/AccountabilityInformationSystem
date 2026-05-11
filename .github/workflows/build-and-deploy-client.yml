name: Client CI

on:
    workflow_run:
        workflows: ["CI - Backend"]
        types:
            - completed
    push:
        branches:
            - main
        paths:
            - 'ais-client/**'

jobs:
    build-and-deploy:
        name: Build
        runs-on: ubuntu-latest
        if: ${{ github.event.workflow_run.conclusion == 'success' || github.event_name == 'push' }}

        defaults:
            run:
                working-directory: ./ais-client
        
        steps:
            - name: Checkout code
              uses: actions/checkout@v4
              with:
                submodules: true
                lfs: true

            - name: Set up Node.js
              uses: actions/setup-node@v4
              with:
                node-version: '22'
                cache: 'npm'
                cache-dependency-path: ./ais-client/package-lock.json

            - name: Install dependencies
              run: npm ci

            #- name: Test
            #  run: npx ng test --watch=false --browsers=ChromeHeadless

            - name: Build
              run: npm run build

            - name: Install SWA CLI
              run: npm install -g @azure/static-web-apps-cli

            - name: Deploy to Azure Static Web Apps
              env:
                AZURE_STATIC_WEB_APPS_API_TOKEN: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
              run: swa deploy --env production --deployment-token $AZURE_STATIC_WEB_APPS_API_TOKEN --output-location dist/ais-client/browser
            