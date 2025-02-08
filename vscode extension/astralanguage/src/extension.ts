import * as vscode from 'vscode';
import * as path from 'path';
import { workspace, ExtensionContext } from 'vscode';

import {
	LanguageClient,
	LanguageClientOptions,
	ServerOptions,
	TransportKind
} from 'vscode-languageclient/node';
  
let client: LanguageClient;

export function activate(context: vscode.ExtensionContext) {
	
	//let serverPath = "C:/Users/REDIZIT/Documents/GitHub/Astra-Rider-extension/LanguageServer/bin/Debug/net8.0/LanguageServer.exe";
	let serverPath = path.join(
		"C:", "Users", "REDIZIT", "Documents", "GitHub", "Astra-Rider-extension",
		"LanguageServer", "bin", "Debug", "net8.0", "LanguageServer.exe"
	);
	

	// The server is implemented in node
	let serverModule = serverPath;
  
	const outputChannel = vscode.window.createOutputChannel("Astra Language Server");

	let serverOptions: ServerOptions = {
	  run: { command: serverModule, transport: TransportKind.stdio },
	  debug: { command: serverModule, transport: TransportKind.stdio }
	};
  
	// Options to control the language client
	let clientOptions: LanguageClientOptions = {
	  documentSelector: [{ scheme: 'file', language: 'Astra' }],
	  synchronize: {
		fileEvents: workspace.createFileSystemWatcher('**/.ac')
	  },
	  outputChannel,
	  traceOutputChannel: outputChannel,
	  markdown:
	  {
		isTrusted: true
	  }
	};
  
	client = new LanguageClient(
	  'languageServerExample',
	  'Language Server Example',
	  serverOptions,
	  clientOptions
	);
	
	client.setTrace(2);
	client.start();
}

export function deactivate(): Thenable<void> | undefined {
	if (!client) {
		return undefined;
	}
	return client.stop();
}