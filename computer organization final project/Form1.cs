using static System.Windows.Forms.DataFormats;

namespace computer_organization_final_project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Set general form design and control styles
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 10);

            // Customize button appearance
            button1.FlatStyle = FlatStyle.Flat;
            button1.BackColor = Color.MediumSlateBlue;
            button1.ForeColor = Color.White;
            button1.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button1.FlatAppearance.BorderSize = 0;
            button1.Cursor = Cursors.Hand;

            // TextBox and ComboBox styling
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;

            // Result display box (not visible if not added to form)
            RichTextBox resultBox = new RichTextBox();
            resultBox.ReadOnly = true;
            resultBox.BackColor = Color.White;
            resultBox.Font = new Font("Consolas", 10);
            resultBox.BorderStyle = BorderStyle.FixedSingle;
        }

        // Decode instruction based on format (binary or hex)
        private string DecodeInstruction(string input, string format)
        {
            uint instruction = 0;

            try
            {
                if (format == "Hexadecimal")
                {
                    if (input.StartsWith("0x")) input = input.Substring(2); // Remove 0x if present
                    instruction = Convert.ToUInt32(input, 16);
                }
                else if (format == "Binary")
                {
                    if (input.Length != 32 || !input.All(c => c == '0' || c == '1'))
                        return "Binary input must be 32 bits and contain only 0-1.";

                    instruction = Convert.ToUInt32(input, 2);
                }
            }
            catch
            {
                return "Invalid input format.";
            }

            // Extract opcode
            uint opcode = instruction & 0x7F;

            // Dispatch based on opcode
            switch (opcode)
            {
                case 0x33: return DecodeRType(instruction); // R-type
                case 0x13:
                case 0x3: return DecodeIType(instruction);   // I-type
                case 0x23: return DecodeSType(instruction);   // S-type
                case 0x63: return DecodeBType(instruction);   // B-type
                case 0x37:
                case 0x17: return DecodeUType(instruction);   // U-type
                case 0x6F: return DecodeJType(instruction);   // J-type
                default: return "Unknown opcode.";
            }
        }

        // R-type instruction decoding (register-register operations)
        private string DecodeRType(uint inst)
        {
            uint rd = (inst >> 7) & 0x1F;
            uint funct3 = (inst >> 12) & 0x7;
            uint rs1 = (inst >> 15) & 0x1F;
            uint rs2 = (inst >> 20) & 0x1F;
            uint funct7 = (inst >> 25) & 0x7F;

            // Check instruction pattern
            if (funct3 == 0x0 && funct7 == 0x00) return $"add x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x0 && funct7 == 0x20) return $"sub x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x4 && funct7 == 0x00) return $"xor x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x6 && funct7 == 0x00) return $"or x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x7 && funct7 == 0x00) return $"and x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x1 && funct7 == 0x00) return $"sll x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x5 && funct7 == 0x00) return $"srl x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x5 && funct7 == 0x20) return $"sra x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x2 && funct7 == 0x00) return $"slt x{rd}, x{rs1}, x{rs2}";
            if (funct3 == 0x3 && funct7 == 0x00) return $"sl x{rd}, x{rs1}, x{rs2}";

            return "Unsupported R-type command.";
        }

        // I-type instruction decoding (immediate)
        private string DecodeIType(uint inst)
        {
            uint rd = (inst >> 7) & 0x1F;
            uint funct3 = (inst >> 12) & 0x7;
            uint rs1 = (inst >> 15) & 0x1F;
            int imm = (int)(inst >> 20); // 12-bit signed immediate
            uint opcode = inst & 0x7F;

            if (opcode == 0x13) // Arithmetic immediate
            {
                if (funct3 == 0x0) return $"addi x{rd}, x{rs1}, {imm}";
                if (funct3 == 0x4) return $"xori x{rd}, x{rs1}, {imm}";
                if (funct3 == 0x6) return $"ori x{rd}, x{rs1}, {imm}";
                if (funct3 == 0x7) return $"andi x{rd}, x{rs1}, {imm}";
                if (funct3 == 0x1) return $"slli x{rd}, x{rs1}, {imm}";
                if (funct3 == 0x5) return $"srli x{rd}, x{rs1}, {imm}";
                if (funct3 == 0x2) return $"slti x{rd}, x{rs1}, {imm}";
                if (funct3 == 0x3) return $"sltiu x{rd}, x{rs1}, {imm}";
            }
            else if (opcode == 0x3) // Load instructions
            {
                if (funct3 == 0x0) return $"lb x{rd}, {imm}(x{rs1})";
                if (funct3 == 0x1) return $"lh x{rd}, {imm}(x{rs1})";
                if (funct3 == 0x2) return $"lw x{rd}, {imm}(x{rs1})";
                if (funct3 == 0x4) return $"lbu x{rd}, {imm}(x{rs1})";
                if (funct3 == 0x5) return $"lhu x{rd}, {imm}(x{rs1})";
            }

            return "Unsupported I-type command.";
        }

        // S-type instruction decoding (store)
        private string DecodeSType(uint inst)
        {
            // Combine two immediate parts
            uint imm4_0 = (inst >> 7) & 0x1F;
            uint imm11_5 = (inst >> 25) & 0x7F;
            int imm = (int)((imm11_5 << 5) | imm4_0);

            uint funct3 = (inst >> 12) & 0x7;
            uint rs1 = (inst >> 15) & 0x1F;
            uint rs2 = (inst >> 20) & 0x1F;

            if (funct3 == 0x0) return $"sb x{rs2}, {imm}(x{rs1})";
            if (funct3 == 0x1) return $"sh x{rs2}, {imm}(x{rs1})";
            if (funct3 == 0x2) return $"sw x{rs2}, {imm}(x{rs1})";

            return "Unsupported S-type command.";
        }

        // B-type instruction decoding (branch)
        private string DecodeBType(uint inst)
        {
            // Construct signed offset from bits
            uint imm = ((inst >> 31) & 0x1) << 12 |
                       ((inst >> 7) & 0x1) << 11 |
                       ((inst >> 25) & 0x3F) << 5 |
                       ((inst >> 8) & 0xF) << 1;
            int offset = (int)imm;
            if ((offset & (1 << 12)) != 0)
                offset |= unchecked((int)0xFFFFE000); // sign extend

            uint funct3 = (inst >> 12) & 0x7;
            uint rs1 = (inst >> 15) & 0x1F;
            uint rs2 = (inst >> 20) & 0x1F;

            if (funct3 == 0x0) return $"beq x{rs1}, x{rs2}, {offset}";
            if (funct3 == 0x1) return $"bne x{rs1}, x{rs2}, {offset}";
            if (funct3 == 0x4) return $"blt x{rs1}, x{rs2}, {offset}";
            if (funct3 == 0x5) return $"bge x{rs1}, x{rs2}, {offset}";
            if (funct3 == 0x6) return $"bltu x{rs1}, x{rs2}, {offset}";
            if (funct3 == 0x7) return $"bgeu x{rs1}, x{rs2}, {offset}";

            return "Unsupported B-type command.";
        }

        // U-type instruction decoding (upper immediate)
        private string DecodeUType(uint inst)
        {
            uint rd = (inst >> 7) & 0x1F;
            int imm = (int)(inst & 0xFFFFF000); // upper 20 bits

            uint opcode = inst & 0x7F;
            if (opcode == 0x37) return $"lui x{rd}, {imm}";
            if (opcode == 0x17) return $"auipc x{rd}, {imm}";

            return "Unsupported U-type command.";
        }

        // J-type instruction decoding (jump)
        private string DecodeJType(uint inst)
        {
            uint rd = (inst >> 7) & 0x1F;
            int imm = (int)(
                ((inst >> 31) & 0x1) << 20 |
                ((inst >> 12) & 0xFF) << 12 |
                ((inst >> 20) & 0x1) << 11 |
                ((inst >> 21) & 0x3FF) << 1
            );

            if ((imm & (1 << 20)) != 0) imm |= unchecked((int)0xFFF00000); // sign extend

            return $"jal x{rd}, {imm}";
        }

        // Form load event: populate combo box
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Hexadecimal");
            comboBox1.Items.Add("Binary");
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;
            string? format = comboBox1.SelectedItem as string;

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(format))
            {
                MessageBox.Show("Please select command and format.");
                return;
            }

            string result = DecodeInstruction(input, format);
            textBox2.Text = result;
            MessageBox.Show(result); // Optional message display
        }

        // (Other empty event handlers omitted)
    }
}