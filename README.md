# CheckCopied v1.0.0

This application helps you manage and analyze multiple React projects.

## Requirements

You need to have installed the following programs globally:

- npx
- yarn

## Installation

You can download the latest version of the application from the [releases](https://github.com/gabymorgi/correccionesUtils/releases) section.

There you will find the user manual and the executable file.

## Process guide

** The following guide is to do a manual process in case you get stuck with the automatic process **

1. Create an empty folder where you want to store the monorepo.

2. Create a subfolder `projects` inside the folder you created in step 1, and clone all the projects you want to manage inside it.

3. Be sure that every project has a the `src` folder in the root directory. Otherwise remove every unnecessary folder.

4. Run Prettier in every project.
for every project do:

create a `.prettierignore` file with the following content:

```
node_modules
build
dist
public
```

create a `.prettierrc.json` file with the following content:

```
{
  "printWidth": 120,
  "tabWidth": 2,
  "semi": false,
  "singleQuote": true,
  "trailingComma": "all",
  "quoteProps": "as-needed",
  "jsxSingleQuote": true,
  "bracketSameLine": false
}
```

Run the following command:

```
npx prettier --write "FOLDER_PATH/src/**/*.{{js,jsx,ts,tsx}}" --ignore-path "FOLDER_PATH\.prettierignore" --config "FOLDER_PATH\.prettierrc.json"
```

replace `FOLDER_PATH` with the path to the project you want to run prettier on.

5. Create a mono repo with yarn

**be sure that every project has a `package.json` file with a unique name**

```
yarn init -y
```

edit the `package.json` file and add the following lines:

```
{
  "name": "react-monorepo",
  "private": true,
  "workspaces": [
    "projects/*"
  ]
}
```

run the following command on the root directory:

```
yarn install
```

to execute a command in every project, run the following command:

```
yarn workspace PROJECT_NAME run COMMAND
```

replace `PROJECT_NAME` with the name of the project you want to run the command on, and `COMMAND` with the command you want to run.

