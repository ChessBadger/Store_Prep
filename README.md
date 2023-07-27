# Prep App - Installation and Usage Guide

## Installation

1. Copy the "Prep_App" folder from my folder on the Z drive to your C drive.

2. Navigate into the "Prep_App" folder and then into the "Release" folder.

3. Open the installer package named "Prep_App" and modify the installation path to `C:\Prep_App\`.

4. Proceed with the installation. If the .NET framework is not installed on your system, you may receive a prompt to install it.

5. After the installation is complete, you will find a new shortcut on your desktop called "Prep App."

## Usage

### First-time setup:

1. Upon opening the "Prep App" for the first time, a new folder named "Prep" will be created on your C drive. Inside this folder, you will find a text file called "accounts" (this will be relevant later).

2. In the main window of the app, you will see a dropdown box and a browse button.

3. Use the dropdown box or browse button to select the file path from which you are copying the prior inventories (e.g., `S:\Scans\Schierl`). Ensure you are in the main folder of the account, where every store number is listed.

4. After selecting the path, a new window will open.

5. In the new window:

   - **Destination**: Browse to the location where you want to paste the prior inventories (usually the folder named "Last" in your setup folder, e.g., `23-07 (xx-xx) Schierl\Last`).

   - **Account Name**: Provide a name for the account (e.g., Schierl, Aurora, etc.). This name can be anything you prefer. Once it is saved, you can use the dropdown during the setup instead of using the browse button.

6. Your chosen account name and the corresponding folder path will be saved in the text file "accounts" on your C drive within the "Prep" folder. The app references this file when you use the dropdown box. You can delete any line from this file to remove it from the dropdown box in the app.

7. Under **Account Format**, select the format in which the stores are listed in the account's folder on the S drive (e.g., Option 1: Kroger, Festival; Option 2: Condon, Aurora, Schierl; Option 3: Gaugert, Maurer's Market).

8. Under **Account Number**, enter the store numbers you want to copy and paste into your "Last" folder. You don't need to include leading zeros for store numbers; for example, 88 will work for store 0088.

9. Press "Enter" and let the program run.

### Program Functionality:

- The program will move all selected folders into the "Last" folder.

- It will remove timesheets from those folders.

- New folders without dates will be created in your main folder.

- The program will move any "To xxxx" folder, "Price" folder, "Cards" folder, and "Teams" file into each of the new folders.

- You should only need to add the dates for upcoming inventories to your folders, and the preparation should be complete.

Please note that the instructions assume you have already installed the app as per the Installation section. If you encounter any issues or have questions, feel free to contact us for support.
