FROM node:20-alpine AS build
WORKDIR /app
COPY src/PeopleDirectory.React/package*.json ./
RUN npm ci
COPY src/PeopleDirectory.React/ .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
