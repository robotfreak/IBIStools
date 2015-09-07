# IBIStools

Programmme zum Senden von Nachrichten an IBIS kompatible (VDV 300 Wagenbus) Anzeigen mit serieller Schnittstelle.

* IBIScmdline Kommandozeilentool. Aufruf 'IBIScmdline COM1 "l123"
* IBISui grafische Oberfläche zur Ansteuerung vin IBIS Linien- bzw. Haltestellen-Anzeigen.

## Features

* derzeit unterstützte Befehle, Liniennummer (DS001),  Sonderzeichen (DS001), 2 Zeilen Zielanzeige (DS003), 4 Zeilen Zielanzeige (DS003, DS003aMAS), Haltestellenanzeige (DS003C, DS009) 
* Emulation einer 1200Baud,7data,even parity 2stoppbits (1200,7,e,2) Schnittstelle (Standard IBIS) für Bluetooth Übertragung (8n1)

## Anforderungen

* Flip-Dot/LED Anzeige mit IBIS Wagenbus-Schnittstelle
* Seriell zu IBIS Schnittstellen Konverter, siehe [IBIS-Wandler](https://github.com/robotfreak/IBIScmdline/tree/master/IBIS-Wandler)
* .NET 4.0, VisualStudio 2015

Eine übersetzte Version der IBISui kann hier heruntergeladen werden:
[IBISui](http://www.robotfreak.de/IBISui/publish.htm)
