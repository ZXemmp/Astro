"""
Servidor HTTP local para servir un build WebGL de Unity.
Lo ejecutas con el botón Play de PyCharm y abre el navegador automáticamente.

- Sirve la carpeta donde está este script.
- Se accede en http://localhost:8000
- Otros dispositivos en la misma WiFi acceden con http://<IP-del-PC>:8000
"""

import http.server
import socketserver
import socket
import webbrowser
import os
from threading import Timer

# === Configuración ===
PORT = 8000
OPEN_BROWSER = True       # Abre el navegador automáticamente
BIND_TO_ALL = True        # Permite acceso desde otros dispositivos en la red local


def get_local_ip():
    """Detecta la IP local del PC en la red WiFi."""
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    try:
        s.connect(("8.8.8.8", 80))
        ip = s.getsockname()[0]
    except Exception:
        ip = "127.0.0.1"
    finally:
        s.close()
    return ip


class UnityWebGLHandler(http.server.SimpleHTTPRequestHandler):
    """
    Handler con headers extra que Unity WebGL puede requerir.
    También evita cacheo molesto al iterar.
    """

    def end_headers(self):
        # Headers requeridos para WebAssembly threads (algunos builds los necesitan)
        self.send_header("Cross-Origin-Opener-Policy", "same-origin")
        self.send_header("Cross-Origin-Embedder-Policy", "require-corp")
        # Evitar caché para ver cambios inmediatamente
        self.send_header("Cache-Control", "no-store, no-cache, must-revalidate")
        super().end_headers()


def main():
    # Servir desde la carpeta donde está este script
    serve_dir = os.path.dirname(os.path.abspath(__file__))
    os.chdir(serve_dir)

    # Verificar que hay un index.html
    if not os.path.exists("index.html"):
        print(f"⚠️  ADVERTENCIA: no se encontró 'index.html' en {serve_dir}")
        print("    Asegúrate de colocar este script en la carpeta del build WebGL.")
        print()

    # Bind: 0.0.0.0 acepta conexiones de la red; 127.0.0.1 solo locales
    bind_address = "0.0.0.0" if BIND_TO_ALL else "127.0.0.1"
    local_ip = get_local_ip()

    print("=" * 60)
    print(" 🚀 Servidor WebGL iniciado")
    print("=" * 60)
    print(f" Sirviendo:   {serve_dir}")
    print(f" Local PC:    http://localhost:{PORT}")
    if BIND_TO_ALL:
        print(f" Red local:   http://{local_ip}:{PORT}")
        print(f"   (úsalo desde tu celular en la misma WiFi)")
    print("=" * 60)
    print(" Pulsa el botón Stop de PyCharm para detener.")
    print()

    # Abrir el navegador después de 1 segundo
    if OPEN_BROWSER:
        Timer(1.0, lambda: webbrowser.open(f"http://localhost:{PORT}")).start()

    try:
        with socketserver.TCPServer((bind_address, PORT), UnityWebGLHandler) as httpd:
            httpd.serve_forever()
    except KeyboardInterrupt:
        print("\n👋 Servidor detenido.")
    except OSError as e:
        if "address already in use" in str(e).lower() or "10048" in str(e):
            print(f"\n❌ El puerto {PORT} ya está en uso.")
            print(f"   Cierra otra ventana de servidor o cambia PORT en el script.")
        else:
            raise


if __name__ == "__main__":
    main()