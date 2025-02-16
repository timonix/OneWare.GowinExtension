# OneWare Studio - GOWIN Toolchain Extension

This extension integrates the **GOWIN FPGA toolchain** into **OneWare Studio**.

## Features
- 🖥️ **Cross-Platform Support**: Tested on **Windows**, should work on **macOS** and **Linux** as well.
- 🔄 **Mixed Language Support**: Supports both **VHDL** and **Verilog** (SystemVerilog is untested).
- 🎯 **Timing Constraints**: Allows adding **timing constraints** via `.sdc` files.
- 🚀 **Simplified Workflow**: Enhances **OneWare Studio** with streamlined FPGA development.

## Installation & Usage
1. Install **OneWare Studio**.
2. Add this extension to enable **GOWIN FPGA** support.
3. Configure your project with `.vhd`, `.vhdl`, `.v`, `.sv`, `.sdc` files.
4. Use the **GOWIN toolchain** for synthesis and programming.

## Notes
- **SystemVerilog support is untested** but may work.
- Contributions and feedback are welcome!

## 🔧 Installation Instructions

### 1️⃣ Add the Custom Source
1. Open **OneWare Studio**.
2. Navigate to **Extras → Settings → Custom Sources**.
3. Add the following URL as a custom source:

https://raw.githubusercontent.com/timonix/OneWare.GowinExtension/main/oneware-extension.json

### 2️⃣ Install the Extension
1. Go to **Extras → Extensions**.
2. Find and install **OneWare GOWIN Extension**.
3. Restart **OneWare Studio** (if needed).

You're now ready to use the **GOWIN FPGA toolchain** inside OneWare Studio! 🚀

[![Test](https://github.com/timonix/OneWare.GowinExtension/actions/workflows/test.yml/badge.svg)](https://github.com/timonix/OneWare.GowinExtension/actions/workflows/test.yml)
[![Publish](https://github.com/timonix/OneWare.GowinExtension/actions/workflows/publish.yml/badge.svg)](https://github.com/timonix/OneWare.GowinExtension/actions/workflows/publish.yml)
