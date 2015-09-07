# IBIStools

Programmme zum Senden von Nachrichten an IBIS kompatible (VDV 300) Flip-Dot Anzeigen (Bus Anzeigen) mit serieller Schnittstelle.

* IBIScmdline Kommandozeilentool. Aufruf 'IBIScmdline COM1 "l123"
* IBISui grafische Oberfläche zur Ansteuerung vin IBIS Linien- bzw. Haltestellen-Anzeigen.

## Features

* derzeit unterstützte Befehle, Liniennummer (lxxx),  Sonderzeichen (lExx), 2 Zeilen Zielanzeige (zA2), 4 Zeilen Zielanzeige (zA4) 
* Emulation einer 7e1 Schnittstelle (Standard IBIS) für Bluetooth Übertragung (8n1)

## Anforderungen

* Flip-Dot/LED Anzeige mit IBIS Bus-Schnittstelle
* Seriell zu IBIS Schnittstellen Konverter, siehe [IBIS-Wandler](https://github.com/robotfreak/IBIScmdline/tree/master/IBIS-Wandler)
* .NET 4.0, VisualStudio 2015

Eine lauffähige Version der IBISui kann hier heruntergeladen werden:
[IBISui](http://www.robotfreal.de/IBISui/publish.htm)
