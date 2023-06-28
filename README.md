# AtlasReaper

AtlasReaper is a command-line tool developed for offensive security purposes, primarily focused on reconnaissance of Confluence and Jira. It also provides various features that can be helpful for tasks such as credential farming and social engineering. The tool is written in C#. 

Blog post: [Sowing Chaos and Reaping Rewards in Confluence and Jira](https://medium.com/specter-ops-posts/sowing-chaos-and-reaping-rewards-in-confluence-and-jira-7a90ba33bf62)

```
                                                   .@@@@
                                               @@@@@
                                            @@@@@   @@@@@@@
                                          @@@@@   @@@@@@@@@@@
                                         @@@@@  @@@@@@@@@@@@@@@
                                        @@@@,  @@@@        *@@@@
                                          @@@@ @@@  @@  @@@ .@@@
   _  _   _         ___                       @@@@@@@     @@@@@@
  /_\| |_| |__ _ __| _ \___ __ _ _ __  ___ _ _   @@   @@@@@@@@
 / _ \  _| / _` (_-<   / -_) _` | '_ \/ -_) '_|  @@   @@@@@@@@
/_/ \_\__|_\__,_/__/_|_\___\__,_| .__/\___|_|    @@@@@@@@   &@
                                |_|             @@@@@@@@@@  @@&
                                                @@@@@@@@@@@@@@@@@
                                               @@@@@@@@@@@@@@@@. @@
                                                  @werdhaihai
```

## Usage

AtlasReaper uses commands, subcommands, and options. The format for executing commands is as follows: 

`.\AtlasReaper.exe [command] [subcommand] [options]`

Replace `[command]`, `[subcommand]`, and `[options]` with the appropriate values based on the action you want to perform. For more information about each command or subcommand, use the `-h` or `--help` option.

Below is a list of available commands and subcommands:

### Commands

Each command has sub commands for interacting with the specific product.

- `confluence`
- `jira`

### Subcommands

#### Confluence

- `confluence attach` - Attach a file to a page.
- `confluence download` - Download an attachment.
- `confluence embed` - Embed a 1x1 pixel image to perform farming attacks.
- `confluence link` - Add a link to a page.
- `confluence listattachments` - List attachments.
- `confluence listpages` - List pages in Confluence.
- `confluence listspaces` - List spaces in Confluence.
- `confluence search` - Search Confluence.

#### Jira

- `jira addcomment` - Add a comment to an issue.
- `jira attach` - Attach a file to an issue.
- `jira createissue` - Create a new issue.
- `jira download` - Download attachment(s) from an issue.
- `jira listattachments` - List attachments on an issue.
- `jira listissues` - List issues in Jira.
- `jira listprojects` - List projects in Jira.
- `jira listusers` - List Atlassian users.
- `jira searchissues` - Search issues in Jira.


#### Common Commands

- `help` - Display more information on a specific command.


## Examples

Here are a few examples of how to use AtlasReaper:

- Search for a keyword in Confluence with wildcard search:
    
    `.\AtlasReaper.exe confluence search --query "http*example.com*" --url $url --cookie $cookie` 
    
- Attach a file to a page in Confluence:
    
    `.\AtlasReaper.exe confluence attach --page-id "12345" --file "C:\path\to\file.exe" --url $url --cookie $cookie`
    
- Create a new issue in Jira:
    
    `.\AtlasReaper.exe jira createissue --project "PROJ" --issue-type Task --message "I can't access this link from my host"  --url $url --cookie $cookie`

## Authentication

Confluence and Jira can be configured to allow anonymous access. You can check this by supplying omitting the -c/--cookie from the commands.

In the event authentication is required, you can dump cookies from a user's browser with [SharpChrome]() or another similar tool.

1. `.\SharpChrome.exe cookies /showall`

2. Look for any cookies scoped to the `*.atlassian.net` named `cloud.session.token` or  `tenant.session.token`

## Limitations

Please note the following limitations of AtlasReaper:

- The tool has not been thoroughly tested in all environments, so it's possible to encounter crashes or unexpected behavior. Efforts have been made to minimize these issues, but caution is advised.
- AtlasReaper uses the `cloud.session.token`  or `tenant.session.token` which can be obtained from a user's browser. Alternatively, it can use anonymous access if permitted. (API tokens or other auth is not currently supported)
- For write operations, the username associated with the user session token (or "anonymous") will be listed.

## Contributing

If you encounter any issues or have suggestions for improvements, please feel free to contribute by submitting a pull request or opening an issue in the [AtlasReaper repo](https://github.com/werdhaihai/AtlasReaper).
