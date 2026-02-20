# RISC-V Instruction Decoder (Computer Organization Project)

This project is a Windows Forms application designed to decode **RISC-V (RV32I)** machine instructions. It was developed as part of the Computer Organization course to visualize how a CPU interprets binary or hexadecimal data.



## 🚀 Features
- **Format Support:** Supports both **Hexadecimal** (e.g., `0x00A502B3`) and **Binary** (32-bit) inputs.
- **Instruction Sets:** Decodes all standard RISC-V instruction types:
  - **R-type:** Arithmetic operations (add, sub, and, or, etc.)
  - **I-type:** Immediate arithmetic and Load operations (addi, lw, etc.)
  - **S-type:** Store operations (sw, sb, sh)
  - **B-type:** Conditional branches (beq, bne, etc.)
  - **U-type:** Upper immediate instructions (lui, auipc)
  - **J-type:** Unconditional jumps (jal)
- **UI/UX:** Modernized WinForms interface with custom styling and error handling.

## 🛠️ Built With
- **Language:** C#
- **Framework:** .NET / Windows Forms
- **Concepts:** Bitwise operations, Instruction Set Architecture (ISA), Computer Architecture.

## 📖 How it Works
1. Select the input format (Hex or Binary).
2. Enter the 32-bit instruction code.
3. The application extracts the **opcode**, **funct3**, **funct7**, and register fields using bitwise shifts (`>>`) and masks (`&`).
4. The decoded assembly instruction is displayed in the result box.

## 🤝 Contribution
This project was developed as a group final project for the Computer Organization course at Düzce University.

---

### 🇹🇷 Proje Hakkında (TR)
Bu uygulama, RISC-V (RV32I) makine komutlarını insan tarafından okunabilir assembly diline dönüştüren bir dekoderdir. Bilgisayar Organizasyonu dersi kapsamında, bir işlemcinin komutları nasıl ayrıştırdığını (parsing) somutlaştırmak amacıyla geliştirilmiştir.
